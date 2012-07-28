using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace Clutch.Diagnostics.EntityFramework
{
	internal abstract class DbTracingProviderFactory : DbProviderFactory
	{ 
	
	}

	internal class DbTracingProviderFactory<T> : DbTracingProviderFactory, IServiceProvider
		 where T : DbProviderFactory
	{
		protected DbTracingProviderFactory()
		{
			var field = typeof(T).GetField("Instance", BindingFlags.Public | BindingFlags.Static);
			if (field == null)
				throw new InvalidOperationException(string.Format("DbProviderFactory '{0}' doesn't have public field 'Instance'", typeof(T).Name));

			this.dbProviderFactory = (T)field.GetValue(null);
		}

		private T dbProviderFactory;

		#region DbProviderFactory

		public override bool CanCreateDataSourceEnumerator
		{
			get
			{
				return dbProviderFactory.CanCreateDataSourceEnumerator;
			}
		}

		public override DbDataSourceEnumerator CreateDataSourceEnumerator()
		{
			return dbProviderFactory.CreateDataSourceEnumerator();
		}

		public override DbCommand CreateCommand()
		{
			return new DbTracingCommand(dbProviderFactory.CreateCommand(), null);
		}

		public override DbConnection CreateConnection()
		{
			return new DbTracingConnection(dbProviderFactory.CreateConnection());
		}

		public override DbParameter CreateParameter()
		{
			return dbProviderFactory.CreateParameter();
		}

		public override DbConnectionStringBuilder CreateConnectionStringBuilder()
		{
			return dbProviderFactory.CreateConnectionStringBuilder();
		}

		public override DbCommandBuilder CreateCommandBuilder()
		{
			return dbProviderFactory.CreateCommandBuilder();
		}

		public override DbDataAdapter CreateDataAdapter()
		{
			return dbProviderFactory.CreateDataAdapter();
		}

		public override CodeAccessPermission CreatePermission(PermissionState state)
		{
			return dbProviderFactory.CreatePermission(state);
		}

		#endregion

		#region IServiceProvider

		/// <summary>
		/// Extension mechanism for additional services;  
		/// </summary>
		/// <returns>requested service provider or null.</returns>
		object IServiceProvider.GetService(Type serviceType)
		{
			var tailProvider = dbProviderFactory as IServiceProvider;
			if (tailProvider == null)
				return null;

			var svc = tailProvider.GetService(serviceType);
			if (svc == null)
				return null;

			if (serviceType == typeof(DbProviderServices))
				svc = new DbTracingProviderServices((DbProviderServices)svc);

			return svc;
		}

		#endregion

		#region Static members

		/// <summary>
		/// Singleton intance.
		/// </summary>
		public static readonly DbTracingProviderFactory<T> Instance = new DbTracingProviderFactory<T>();

		#endregion
	}
}
