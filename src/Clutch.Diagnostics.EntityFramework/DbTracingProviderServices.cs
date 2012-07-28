using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Clutch.Diagnostics.EntityFramework
{
	public class DbTracingProviderServices : DbProviderServices
	{
		public DbTracingProviderServices(DbProviderServices providerServices)
		{
			this.providerServices = providerServices;
		}

		private DbProviderServices providerServices;

		private static DbConnection GetRealConnection(DbConnection connection)
		{
			var tracingConnection = connection as DbTracingConnection;

			if (tracingConnection != null)
				return tracingConnection.UnderlyingConnection;

			return connection;
		}

		#region DbProviderServices

		protected override DbProviderManifest GetDbProviderManifest(string manifestToken)
		{
			return providerServices.GetProviderManifest(manifestToken);
		}

		protected override string GetDbProviderManifestToken(DbConnection connection)
		{
			return providerServices.GetProviderManifestToken(GetRealConnection(connection));
		}

		protected override DbCommandDefinition CreateDbCommandDefinition(DbProviderManifest providerManifest, System.Data.Common.CommandTrees.DbCommandTree commandTree)
		{
			var cmdDef = providerServices.CreateCommandDefinition(providerManifest, commandTree);
			var cmd = cmdDef.CreateCommand();
			return CreateCommandDefinition(new DbTracingCommand(cmd, cmd.Connection));
		}

		protected override void DbCreateDatabase(DbConnection connection, int? commandTimeout, System.Data.Metadata.Edm.StoreItemCollection storeItemCollection)
		{
			providerServices.CreateDatabase(GetRealConnection(connection), commandTimeout, storeItemCollection);
		}

		protected override void DbDeleteDatabase(DbConnection connection, int? commandTimeout, System.Data.Metadata.Edm.StoreItemCollection storeItemCollection)
		{
			providerServices.DeleteDatabase(GetRealConnection(connection), commandTimeout, storeItemCollection);
		}

		protected override string DbCreateDatabaseScript(string providerManifestToken, System.Data.Metadata.Edm.StoreItemCollection storeItemCollection)
		{
			return providerServices.CreateDatabaseScript(providerManifestToken, storeItemCollection);
		}

		protected override bool DbDatabaseExists(DbConnection connection, int? commandTimeout, System.Data.Metadata.Edm.StoreItemCollection storeItemCollection)
		{
			return providerServices.DatabaseExists(GetRealConnection(connection), commandTimeout, storeItemCollection);
		}

		public override DbCommandDefinition CreateCommandDefinition(DbCommand prototype)
		{
			return providerServices.CreateCommandDefinition(prototype);
		}

		#endregion
	}
}
