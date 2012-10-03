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
		[Obsolete("This method is now invoked by Clutch.Bootstrap.Startup call, will be non-public in near future")]
		public static void Initialize()
		{ }

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
