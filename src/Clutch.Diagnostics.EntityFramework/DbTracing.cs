using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Data.Common;
using System.Data.EntityClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EFProviderWrapperToolkit;
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
		public static void Initialize(string wrappedProviderName = "System.Data.SqlClient")
		{
			DbTracingProviderFactory.SetWrappedProviderName(wrappedProviderName);
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

		internal static void FireCommandExecuting(DbConnection connection, DbTracingCommand command)
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

		internal static void FireCommandFinished(DbConnection connection, DbTracingCommand command, object result, TimeSpan duration)
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

		internal static void FireCommandFailed(DbConnection connection, DbTracingCommand command, Exception ex, TimeSpan duration)
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

		private static void FireCommandExecuted(DbConnection connection, DbTracingCommand command, object result, TimeSpan duration)
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
