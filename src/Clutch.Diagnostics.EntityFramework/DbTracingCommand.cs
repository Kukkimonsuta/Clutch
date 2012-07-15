using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EFProviderWrapperToolkit;

namespace Clutch.Diagnostics.EntityFramework
{
	/// <summary>
	/// Wraps command and fires connection events.
	/// </summary>
	public class DbTracingCommand : DbCommandWrapper
	{
		internal DbTracingCommand(DbCommand command)
			: this(command, null)
		{ }
		internal DbTracingCommand(DbCommand command, DbCommandDefinitionWrapper definition)
			: base(command, definition)
		{
			this.Id = Guid.NewGuid();
		}

		/// <summary>
		/// Returns unique id of the command.
		/// </summary>
		public Guid Id { get; protected set; }

		#region DbCommandWrapper

		/// <summary>
		/// Executes the query and returns the first column of the first row in the result set returned by the query. All other columns and rows are ignored.
		/// </summary>
		public override object ExecuteScalar()
		{
			var sw = new Stopwatch();

			DbTracing.FireCommandExecuting(Connection, this);
			try
			{
				sw.Start();
				var result = base.ExecuteScalar();
				sw.Stop();
				DbTracing.FireCommandFinished(Connection, this, result, sw.Elapsed);
				return result;
			}
			catch (Exception ex)
			{
				sw.Stop();

				DbTracing.FireCommandFailed(Connection, this, ex, sw.Elapsed);
				throw;
			}
		}

		/// <summary>
		/// Executes a SQL statement against a connection object.
		/// </summary>
		public override int ExecuteNonQuery()
		{
			var sw = new Stopwatch();

			DbTracing.FireCommandExecuting(Connection, this);
			try
			{
				sw.Start();
				var result = base.ExecuteNonQuery();
				sw.Stop();
				DbTracing.FireCommandFinished(Connection, this, result, sw.Elapsed);
				return result;
			}
			catch (Exception ex)
			{
				DbTracing.FireCommandFailed(Connection, this, ex, sw.Elapsed);
				throw;
			}
		}

		/// <summary>
		/// Executes the command text against the connection.
		/// </summary>
		protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
		{
			var sw = new Stopwatch();

			DbTracing.FireCommandExecuting(Connection, this);
			try
			{
				sw.Start();
				var result = base.ExecuteDbDataReader(behavior);
				sw.Stop();
				DbTracing.FireCommandFinished(Connection, this, result, sw.Elapsed);
				return result;
			}
			catch (Exception ex)
			{
				sw.Stop();

				DbTracing.FireCommandFailed(Connection, this, ex, sw.Elapsed);
				throw;
			}
		}

		#endregion
	}
}
