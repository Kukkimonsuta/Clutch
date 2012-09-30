using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Clutch.Runtime
{
	public class LocalDataStoreScopeStrategy : IExecutionScopeStrategy
	{
		private LocalDataStoreSlot scopeSlot = Thread.AllocateDataSlot();

		public void Set(object scope)
		{
			Thread.SetData(scopeSlot, scope);
		}

		public object Get()
		{
			return Thread.GetData(scopeSlot);
		}
	}
}
