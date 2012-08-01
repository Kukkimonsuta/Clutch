using System;
using System.Data;
using System.Data.Common;
using System.Diagnostics;

namespace Clutch.Diagnostics.EntityFramework
{
	public class DbTracingCommand : DbCommand, ICloneable
	{
		public DbTracingCommand(DbCommand command, DbConnection connection)
		{
			if (command == null)
				throw new ArgumentNullException("command");
			if (command is DbTracingCommand)
				throw new InvalidOperationException("Command is already wrapped");

			this.command = command;
			this.connection = connection;
		}

		private DbCommand command;
		private DbConnection connection;

		public DbCommand UnderlyingCommand
		{
			get
			{
				return command;
			}
		}

		#region DbCommand

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities",
			Justification = "This is just a wrapper over DbCommand")]
		public override string CommandText
		{
			get { return command.CommandText; }
			set { command.CommandText = value; }
		}

		public override int CommandTimeout
		{
			get { return command.CommandTimeout; }
			set { command.CommandTimeout = value; }
		}

		public override CommandType CommandType
		{
			get { return command.CommandType; }
			set { command.CommandType = value; }
		}

		protected override DbConnection DbConnection
		{
			get
			{
				return connection;
			}
			set
			{
				connection = value;

				var tracingConnection = value as DbTracingConnection;
				if (tracingConnection != null)
					command.Connection = tracingConnection.UnderlyingConnection;
				else
					command.Connection = value;
			}
		}

		protected override DbParameterCollection DbParameterCollection
		{
			get
			{
				return command.Parameters;
			}
		}

		protected override DbTransaction DbTransaction
		{
			get { return command.Transaction; }
			set { command.Transaction = value; }
		}

		public override bool DesignTimeVisible
		{
			get { return command.DesignTimeVisible; }
			set { command.DesignTimeVisible = value; }
		}

		public override UpdateRowSource UpdatedRowSource
		{
			get { return command.UpdatedRowSource; }
			set { command.UpdatedRowSource = value; }
		}

		protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
		{
			var sw = new Stopwatch();

			DbTracing.FireCommandExecuting(Connection, this);
			try
			{
				sw.Start();
				var result = command.ExecuteReader(behavior);
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

		public override int ExecuteNonQuery()
		{
			var sw = new Stopwatch();

			DbTracing.FireCommandExecuting(Connection, this);
			try
			{
				sw.Start();
				var result = command.ExecuteNonQuery();
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

		public override object ExecuteScalar()
		{
			var sw = new Stopwatch();

			DbTracing.FireCommandExecuting(Connection, this);
			try
			{
				sw.Start();
				var result = command.ExecuteScalar();
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

		public override void Cancel()
		{
			command.Cancel();
		}

		public override void Prepare()
		{
			command.Prepare();
		}

		protected override DbParameter CreateDbParameter()
		{
			return command.CreateParameter();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && command != null)
			{
				command.Dispose();
			}

			command = null;
			base.Dispose(disposing);
		}

		#endregion

		#region ICloneable

		public DbTracingCommand Clone()
		{
			ICloneable tail = command as ICloneable;

			if (tail == null)
				throw new NotSupportedException("Underlying " + command.GetType().Name + " is not cloneable");

			return new DbTracingCommand((DbCommand)tail.Clone(), connection);
		}

		object ICloneable.Clone()
		{
			return Clone();
		}

		#endregion
	}
}
