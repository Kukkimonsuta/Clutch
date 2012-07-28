using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Clutch.Diagnostics.Logging.Interceptors;

namespace Clutch.Diagnostics.Logging
{
	public static class LogEventInterceptorProviders
	{
		private static readonly LogEventInterceptorProviderCollection providers = new LogEventInterceptorProviderCollection()
		{
			new DefaultLogEventInterceptorProvider()
		};

		public static LogEventInterceptorProviderCollection Providers
		{
			get { return providers; }
		}
	}
}
