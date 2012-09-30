using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clutch.Runtime
{
	public interface IExecutionScopeBehavior
	{
		void ScopeOpen(object scope);
		void ScopeClose(object scope);
	}
}
