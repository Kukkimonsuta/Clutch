using System;
using System.Data.Common;

namespace Clutch.Diagnostics.EntityFramework
{
	public interface IDbTracingListener
	{
		void CommandExecuting(DbConnection connection, DbCommand command);
		void CommandFinished(DbConnection connection, DbCommand command, object result, TimeSpan duration);
		void CommandFailed(DbConnection connection, DbCommand command, Exception exception, TimeSpan duration);
		void CommandExecuted(DbConnection connection, DbCommand command, object result, TimeSpan duration);
	}
}
