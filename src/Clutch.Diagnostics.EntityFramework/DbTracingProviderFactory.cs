using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EFProviderWrapperToolkit;

namespace Clutch.Diagnostics.EntityFramework
{
	/// <summary>
	/// Factory of db tracing provider.
	/// </summary>
	internal class DbTracingProviderFactory : DbProviderFactoryBase
	{
		public const string InvariantProviderName = "Clutch.Diagnostics.EntityFramework.DbTracingProvider";

		private DbTracingProviderFactory(DbProviderServices providerServices, string wrappedProviderName = "System.Data.SqlClient")
			: base(providerServices)
		{
			WrappedProviderName = wrappedProviderName;
		}

		public string WrappedProviderName { get; private set; }

		#region DbProviderFactoryBase

		/// <summary>
		/// Creates new tracing connection.
		/// </summary>
		public override DbConnection CreateConnection()
		{
			var factory = DbProviderFactories.GetFactory(WrappedProviderName);

			return new DbTracingConnection(factory.CreateConnection());
		}

		#endregion

		#region Static members

		static DbTracingProviderFactory()
		{
			DbProviderFactoryBase.RegisterProvider("Db Tracing Data Provider", InvariantProviderName, "Tracing Data Provider", typeof(DbTracingProviderFactory));
		}

		/// <summary>
		/// Gets the singleton instance.
		/// </summary>
		public static readonly DbTracingProviderFactory Instance = new DbTracingProviderFactory(DbTracingProviderServices.Instance);

		public static void SetWrappedProviderName(string providerName)
		{
			Instance.WrappedProviderName = providerName;
		}

		#endregion
	}
}
