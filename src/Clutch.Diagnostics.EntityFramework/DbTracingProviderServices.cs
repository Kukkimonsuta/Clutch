using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Common.CommandTrees;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EFProviderWrapperToolkit;

namespace Clutch.Diagnostics.EntityFramework
{
	/// <summary>
	/// Implementation of <see cref="DbProviderServices"/> for EFTracingProvider.
	/// </summary>
	internal class DbTracingProviderServices : DbProviderServicesBase
	{
		private DbTracingProviderServices()
		{ }

		#region DbProviderServicesBase

		/// <summary>
		/// Gets the default name of the wrapped provider.
		/// </summary>
		protected override string DefaultWrappedProviderName
		{
			get { return DbTracingProviderFactory.Instance.WrappedProviderName; }
		}

		/// <summary>
		/// Gets the provider invariant iname.
		/// </summary>
		protected override string ProviderInvariantName
		{
			get { return DbTracingProviderFactory.InvariantProviderName; }
		}

		/// <summary>
		/// Creates the command definition wrapper.
		/// </summary>
		public override DbCommandDefinitionWrapper CreateCommandDefinitionWrapper(DbCommandDefinition wrappedCommandDefinition, DbCommandTree commandTree)
		{
			return new DbCommandDefinitionWrapper(wrappedCommandDefinition, commandTree, (cmd, def) => new DbTracingCommand(cmd, def));
		}

		#endregion

		#region Static members

		static DbTracingProviderServices()
		{
			Instance = new DbTracingProviderServices();
		}

		/// <summary>
		/// Gets the singleton instance.
		/// </summary>
		public static DbTracingProviderServices Instance { get; private set; }

		#endregion
	}
}
