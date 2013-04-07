using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Clutch.Diagnostics.EntityFramework
{
    /// <summary>
    /// Helper for creating traced connections.
    /// </summary>
    public static class DbTracing
    {
        /// <summary>
        /// Intializes tracing
        /// </summary>
        private static void Initialize()
        {
            if (initialized)
                return;

            DbProviderFactories.GetFactoryClasses();

            var type = typeof(DbProviderFactories);

            DataTable table;
            object setOrTable = (type.GetField("_configTable", BindingFlags.NonPublic | BindingFlags.Static) ?? type.GetField("_providerTable", BindingFlags.NonPublic | BindingFlags.Static)).GetValue(null);
            if (setOrTable is DataSet)
            {
                table = ((DataSet)setOrTable).Tables["DbProviderFactories"];
            }

            table = (DataTable)setOrTable;

            foreach (var row in table.Rows.Cast<DataRow>().ToList())
            {
                DbProviderFactory factory;
                try
                {
                    factory = DbProviderFactories.GetFactory(row);
                }
                catch (Exception)
                {
                    continue;
                }

                // this provider is already wrapped
                if (factory is DbTracingProviderFactory)
                    continue;

                var profiledType = typeof(DbTracingProviderFactory<>).MakeGenericType(factory.GetType());
                if (profiledType != null)
                {
                    var profiled = table.NewRow();
                    profiled["Name"] = row["Name"];
                    profiled["Description"] = row["Description"];
                    profiled["InvariantName"] = row["InvariantName"];
                    profiled["AssemblyQualifiedName"] = profiledType.AssemblyQualifiedName;
                    table.Rows.Remove(row);
                    table.Rows.Add(profiled);
                }
            }

            initialized = true;
        }

        private static ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        private static IList<IDbTracingListener> listeners = new List<IDbTracingListener>();
        private static bool initialized = false;
        private static volatile bool enabled = false;

        public static bool IsEnabled
        {
            get { return enabled; }
        }

        public static void AddListener(IDbTracingListener listener)
        {
            rwLock.EnterWriteLock();
            try
            {
                listeners.Add(listener);
            }
            finally
            {
                rwLock.ExitWriteLock();
            }
        }

        public static void AddListeners(params IDbTracingListener[] listeners)
        {
            rwLock.EnterWriteLock();
            try
            {
                foreach (var listener in listeners)
                    DbTracing.listeners.Add(listener);
            }
            finally
            {
                rwLock.ExitWriteLock();
            }
        }

        public static void RemoveListener(IDbTracingListener listener)
        {
            rwLock.EnterWriteLock();
            try
            {
                listeners.Remove(listener);
            }
            finally
            {
                rwLock.ExitWriteLock();
            }
        }

        public static void Enable(params IDbTracingListener[] listeners)
        {
            Initialize();

            AddListeners(listeners);

            enabled = true;
        }

        public static void Disable()
        {
            enabled = false;
        }

        internal static void FireCommandExecuting(DbTracingContext context)
        {
            if (!enabled)
                return;

            rwLock.EnterReadLock();
            try
            {
                foreach (var listener in listeners)
                    listener.CommandExecuting(context);
            }
            finally
            {
                rwLock.ExitReadLock();
            }
        }

        internal static void FireCommandFinished(DbTracingContext context)
        {
            if (!enabled)
                return;

            rwLock.EnterReadLock();
            try
            {
                foreach (var listener in listeners)
                    listener.CommandFinished(context);
            }
            finally
            {
                rwLock.ExitReadLock();
            }

            if (context.Type != DbTracingType.Reader)
                FireCommandExecuted(context);
        }

        internal static void FireCommandFailed(DbTracingContext context)
        {
            if (!enabled)
                return;

            rwLock.EnterReadLock();
            try
            {
                foreach (var listener in listeners)
                    listener.CommandFailed(context);
            }
            finally
            {
                rwLock.ExitReadLock();
            }

            FireCommandExecuted(context);
        }

        internal static void FireReaderFinished(DbTracingContext context)
        {
            if (!enabled)
                return;

            rwLock.EnterReadLock();
            try
            {
                foreach (var listener in listeners)
                    listener.ReaderFinished(context);
            }
            finally
            {
                rwLock.ExitReadLock();
            }

            FireCommandExecuted(context);
        }

        private static void FireCommandExecuted(DbTracingContext context)
        {
            if (!enabled)
                return;

            rwLock.EnterReadLock();
            try
            {
                foreach (var listener in listeners)
                    listener.CommandExecuted(context);
            }
            finally
            {
                rwLock.ExitReadLock();
            }
        }
    }
}
