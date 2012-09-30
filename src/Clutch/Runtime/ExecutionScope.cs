using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clutch.Runtime
{
	public sealed class ExecutionScope : TemporaryActionBase
	{
		public ExecutionScope()
		{
			parentScope = Strategy.Get();
			Strategy.Set(this);

			foreach (var behavior in Behaviors)
				behavior.ScopeOpen(this);
		}

		private object parentScope;

		protected override void Revert()
		{
			var currentScope = CurrentScope;
			if (currentScope != this)
				throw new InvalidOperationException("Attempting to dispose foreign scope");

			foreach (var behavior in Behaviors)
				behavior.ScopeClose(this);

			Strategy.Set(parentScope);
		}

		#region Static members

		static ExecutionScope()
		{
			Strategy = new LocalDataStoreScopeStrategy();
			Behaviors = new List<IExecutionScopeBehavior>();
		}

		public static IExecutionScopeStrategy Strategy { get; set; }
		public static IList<IExecutionScopeBehavior> Behaviors { get; private set; }

		public static object CurrentScope { get { return Strategy.Get(); } }

		#endregion
	}
}
