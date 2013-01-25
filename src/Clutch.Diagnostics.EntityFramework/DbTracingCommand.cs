using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;

namespace Clutch.Diagnostics.EntityFramework
{
#if !DEBUG
	[DebuggerStepThrough]
#endif
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
            if (!DbTracing.IsEnabled)
                return command.ExecuteReader(behavior);

            var context = new DbTracingContext(DbTracingType.Reader, connection, command);

            DbTracing.FireCommandExecuting(context);
            DbTracingDataReader result = null;
            try
            {
                context.OnStarted();
                result = new DbTracingDataReader(command.ExecuteReader(behavior), context);
                context.OnFinished(result.UnderlyingReader);
            }
            catch (Exception ex)
            {
                context.OnFailed(ex);

                DbTracing.FireCommandFailed(context);
                throw;
            }

            DbTracing.FireCommandFinished(context);

            return result;
        }

        public override int ExecuteNonQuery()
        {
            if (!DbTracing.IsEnabled)
                return command.ExecuteNonQuery();

            var context = new DbTracingContext(DbTracingType.NonQuery, connection, command);

            DbTracing.FireCommandExecuting(context);
            int result = 0;
            try
            {
                context.OnStarted();
                result = command.ExecuteNonQuery();
                context.OnFinished(result);
            }
            catch (Exception ex)
            {
                context.OnFailed(ex);

                DbTracing.FireCommandFailed(context);
                throw;
            }

            DbTracing.FireCommandFinished(context);

            return result;
        }

        public override object ExecuteScalar()
        {
            if (!DbTracing.IsEnabled)
                return command.ExecuteScalar();

            var context = new DbTracingContext(DbTracingType.Scalar, connection, command);

            object result = null;
            DbTracing.FireCommandExecuting(context);
            try
            {
                context.OnStarted();
                result = command.ExecuteScalar();
                context.OnFinished(result);
            }
            catch (Exception ex)
            {
                context.OnFailed(ex);

                DbTracing.FireCommandFailed(context);
                throw;
            }

            DbTracing.FireCommandFinished(context);

            return result;
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
