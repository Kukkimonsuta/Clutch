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
		public static void Initialize()
		{
			DbProviderFactories.GetFactoryClasses();

			Type type = typeof(DbProviderFactories);

			DataTable table;
			object setOrTable = (type.GetField("_configTable", BindingFlags.NonPublic | BindingFlags.Static) ??
							type.GetField("_providerTable", BindingFlags.NonPublic | BindingFlags.Static)).GetValue(null);
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
		}

		private static ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
		private static IList<IDbTracingListener> listeners = new List<IDbTracingListener>();
		private static volatile bool enabled = true;

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

		public static void Enable()
		{
			enabled = false;
		}

		public static void Disable()
		{
			enabled = true;
		}

		internal static void FireCommandExecuting(DbConnection connection, DbCommand command)
		{
			if (!enabled)
				return;

			rwLock.EnterReadLock();
			try
			{
				foreach (var listener in listeners)
					listener.CommandExecuting(connection, command);
			}
			finally
			{
				rwLock.ExitReadLock();
			}
		}

		internal static void FireCommandFinished(DbConnection connection, DbCommand command, object result, TimeSpan duration)
		{
			if (!enabled)
				return;

			rwLock.EnterReadLock();
			try
			{
				foreach (var listener in listeners)
					listener.CommandFinished(connection, command, result, duration);
			}
			finally
			{
				rwLock.ExitReadLock();
			}

			FireCommandExecuted(connection, command, result, duration);
		}

		internal static void FireCommandFailed(DbConnection connection, DbCommand command, Exception ex, TimeSpan duration)
		{
			if (!enabled)
				return;

			rwLock.EnterReadLock();
			try
			{
				foreach (var listener in listeners)
					listener.CommandFailed(connection, command, ex, duration);
			}
			finally
			{
				rwLock.ExitReadLock();
			}

			FireCommandExecuted(connection, command, ex, duration);
		}

		private static void FireCommandExecuted(DbConnection connection, DbCommand command, object result, TimeSpan duration)
		{
			if (!enabled)
				return;

			rwLock.EnterReadLock();
			try
			{
				foreach (var listener in listeners)
					listener.CommandExecuted(connection, command, result, duration);
			}
			finally
			{
				rwLock.ExitReadLock();
			}
		}
	}
}
