using Clutch.Runtime;
using Clutch.Diagnostics.Logging;
using Clutch.Web.Diagnostics.Logging.Interceptors;

namespace Clutch.Web.Diagnostics.Logging
{
	internal class LoggingBootstrap : BootstrapSubscriber
	{
		public override void Startup()
		{
			LogEventInterceptorProviders.Providers.Add(new WebLogEventInterceptorProvider());
		}
	}
}
