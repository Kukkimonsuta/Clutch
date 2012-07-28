using System.Collections.Generic;
using Clutch.Diagnostics.Logging;

namespace Clutch.Web.Diagnostics.Logging.Interceptors
{
	public class DefaultLogEventInterceptorProvider : LogEventInterceptorProvider
	{
		public override IEnumerable<ILogEventInterceptor> GetInterceptors()
		{
			yield return RequestInterceptor.Instance;
		}
	}
}
