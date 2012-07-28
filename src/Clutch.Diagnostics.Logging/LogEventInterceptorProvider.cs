using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clutch.Diagnostics.Logging
{
	public abstract class LogEventInterceptorProvider
	{
		public abstract IEnumerable<ILogEventInterceptor> GetInterceptors();
	}
}
