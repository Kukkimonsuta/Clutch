using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clutch.Runtime
{
	public interface IExecutionScopeStrategy
	{
		void Set(object scope);
		object Get();
	}
}
