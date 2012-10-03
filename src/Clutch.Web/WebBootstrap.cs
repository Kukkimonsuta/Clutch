using Clutch.Runtime;
using Clutch.Web.Runtime;

namespace Clutch.Web.Diagnostics.Logging
{
	internal class WebBootstrap : BootstrapSubscriber
	{
		public override void Startup()
		{
			ExecutionScope.Strategy = new HttpContextScopeStrategy();
		}
	}
}
