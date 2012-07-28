using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clutch.Diagnostics.Logging.Interceptors
{
	public class DefaultLogEventInterceptorProvider : LogEventInterceptorProvider
	{
		public override IEnumerable<ILogEventInterceptor> GetInterceptors()
		{
			yield return MessageIdInterceptor.Instance;
			yield return RunIdInterceptor.Instance;
			yield return IdentityInterceptor.Instance;
		}
	}
}
