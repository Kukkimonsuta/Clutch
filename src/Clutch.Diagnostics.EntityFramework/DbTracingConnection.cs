using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EFProviderWrapperToolkit;
using System.Data.Common;

namespace Clutch.Diagnostics.EntityFramework
{
	/// <summary>
	/// Wraps connection and holds events.
	/// </summary>
	public class DbTracingConnection : DbConnectionWrapper
	{
		internal DbTracingConnection(DbConnection wrappedConnection)
			: base()
		{
			this.WrappedConnection = wrappedConnection;
		}

		#region DbConnectionWrapper

		protected override string DefaultWrappedProviderName
		{
			get { return DbTracingProviderFactory.Instance.WrappedProviderName; }
		}

		protected override DbProviderFactory DbProviderFactory
		{
			get { return DbTracingProviderFactory.Instance; }
		}

		/// <summary>
		/// Creates and returns a System.Data.Common.DbCommand object associated with the current connection.
		/// </summary>
		protected override DbCommand CreateDbCommand()
		{
			var command = WrappedConnection.CreateCommand();

			return new DbTracingCommand(command) { Connection = this };
		}

		#endregion
	}
}
