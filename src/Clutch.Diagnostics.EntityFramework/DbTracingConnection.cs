using System;
using System.Data;
using System.Data.Common;
using System.Reflection;

namespace Clutch.Diagnostics.EntityFramework
{
	public class DbTracingConnection : DbConnection, ICloneable
	{
		public DbTracingConnection(DbConnection connection)
		{
			if (connection == null)
				throw new ArgumentNullException("connection");
			if (connection is DbTracingConnection)
				throw new InvalidOperationException("Connection is already wrapped");

			this.connection = connection;
			this.connection.StateChange += StateChangeHandler;
		}

		private DbConnection connection;
		private DbProviderFactory factory;

		public DbConnection UnderlyingConnection
		{
			get { return connection; }
		}

		private void StateChangeHandler(object sender, StateChangeEventArgs e)
		{
			OnStateChange(e);
		}

		#region DbConnection

		protected override DbProviderFactory DbProviderFactory
		{
			get
			{
				if (factory != null) 
					return factory;

				var tail = ripInnerProvider(connection);
				factory = (DbProviderFactory)typeof(DbTracingProviderFactory<>).MakeGenericType(tail.GetType())
					.GetField("Instance", BindingFlags.Public | BindingFlags.Static)
					.GetValue(null);
				return factory;
			}
		}

		protected override bool CanRaiseEvents
		{
			get { return true; }
		}

		public override string ConnectionString
		{
			get { return connection.ConnectionString; }
			set { connection.ConnectionString = value; }
		}

		public override int ConnectionTimeout
		{
			get { return connection.ConnectionTimeout; }
		}

		public override string Database
		{
			get { return connection.Database; }
		}

		public override string DataSource
		{
			get { return connection.DataSource; }
		}

		public override string ServerVersion
		{
			get { return connection.ServerVersion; }
		}

		public override ConnectionState State
		{
			get { return connection.State; }
		}

		public override void ChangeDatabase(string databaseName)
		{
			connection.ChangeDatabase(databaseName);
		}

		public override void Close()
		{
			connection.Close();
		}

		public override void EnlistTransaction(System.Transactions.Transaction transaction)
		{
			connection.EnlistTransaction(transaction);
		}

		public override DataTable GetSchema()
		{
			return connection.GetSchema();
		}

		public override DataTable GetSchema(string collectionName)
		{
			return connection.GetSchema(collectionName);
		}

		public override DataTable GetSchema(string collectionName, string[] restrictionValues)
		{
			return connection.GetSchema(collectionName, restrictionValues);
		}

		public override void Open()
		{
			connection.Open();
		}

		protected override DbTransaction BeginDbTransaction(System.Data.IsolationLevel isolationLevel)
		{
			return connection.BeginTransaction(isolationLevel);
		}

		protected override DbCommand CreateDbCommand()
		{
			return new DbTracingCommand(connection.CreateCommand(), this);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && connection != null)
			{
				connection.StateChange -= StateChangeHandler;
				connection.Dispose();
			}

			connection = null;

			base.Dispose(disposing);
		}

		#endregion

		#region ICloneable

		public DbTracingConnection Clone()
		{
			var tail = connection as ICloneable;

			if (tail == null) 
				throw new NotSupportedException("Underlying " + connection.GetType().Name + " is not cloneable");

			return new DbTracingConnection((DbConnection)tail.Clone());
		}

		object ICloneable.Clone()
		{
			return Clone();
		}

		#endregion

		#region Static members

		private static readonly Func<DbConnection, DbProviderFactory> ripInnerProvider = (Func<DbConnection, DbProviderFactory>)Delegate.CreateDelegate(
			typeof(Func<DbConnection, DbProviderFactory>),
			typeof(DbConnection).GetProperty("DbProviderFactory", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).GetGetMethod(true)
		);

		#endregion
	}
}
