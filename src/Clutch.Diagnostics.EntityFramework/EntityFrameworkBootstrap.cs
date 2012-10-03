using Clutch.Runtime;
using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;

namespace Clutch.Diagnostics.EntityFramework
{
	internal class EntityFrameworkBootstrap : BootstrapSubscriber
	{
		public override void Startup()
		{
			DbProviderFactories.GetFactoryClasses();

			var type = typeof(DbProviderFactories);

			DataTable table;
			object setOrTable = (type.GetField("_configTable", BindingFlags.NonPublic | BindingFlags.Static) ??
							type.GetField("_providerTable", BindingFlags.NonPublic | BindingFlags.Static)).GetValue(null);
			if (setOrTable is DataSet)
			{
				table = ((DataSet)setOrTable).Tables["DbProviderFactories"];
			}

			table = (DataTable)setOrTable;

			foreach (var row in table.Rows.Cast<DataRow>().ToList())
			{
				DbProviderFactory factory;
				try
				{
					factory = DbProviderFactories.GetFactory(row);
				}
				catch (Exception)
				{
					continue;
				}

				// this provider is already wrapped
				if (factory is DbTracingProviderFactory)
					continue;

				var profiledType = typeof(DbTracingProviderFactory<>).MakeGenericType(factory.GetType());
				if (profiledType != null)
				{
					var profiled = table.NewRow();
					profiled["Name"] = row["Name"];
					profiled["Description"] = row["Description"];
					profiled["InvariantName"] = row["InvariantName"];
					profiled["AssemblyQualifiedName"] = profiledType.AssemblyQualifiedName;
					table.Rows.Remove(row);
					table.Rows.Add(profiled);
				}
			}
		}
	}
}
