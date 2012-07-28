using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clutch.Diagnostics.Logging
{
	public class LogEventInterceptorProviderCollection : Collection<LogEventInterceptorProvider>
	{
		internal LogEventInterceptorProviderCollection(params LogEventInterceptorProvider[] providers)
			: base(providers.ToList())
		{ }

		protected override void InsertItem(int index, LogEventInterceptorProvider provider)
		{
			if (provider == null)
				throw new ArgumentNullException("provider");

			base.InsertItem(index, provider);
		}

		protected override void SetItem(int index, LogEventInterceptorProvider provider)
		{
			if (provider == null)
				throw new ArgumentNullException("provider");

			base.SetItem(index, provider);
		}

		public IEnumerable<ILogEventInterceptor> GetInterceptors()
		{
			return this.SelectMany(p => p.GetInterceptors()).Where(p => p != null);
		}
	}
}
