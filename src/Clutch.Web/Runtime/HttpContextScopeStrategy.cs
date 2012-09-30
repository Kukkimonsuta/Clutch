using Clutch.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Clutch.Web.Runtime
{
	public class HttpContextScopeStrategy : IExecutionScopeStrategy
	{
		public HttpContextScopeStrategy()
		{
			key = typeof(HttpContextScopeStrategy).FullName + ":" + Guid.NewGuid();
		}

		private string key;

		public void Set(object scope)
		{
			if (HttpContext.Current == null)
				throw new InvalidOperationException("Cannot set scope outside of http request");

			HttpContext.Current.Items[key] = scope;
		}

		public object Get()
		{
			if (HttpContext.Current == null)
				throw new InvalidOperationException("Cannot get scope outside of http request");

			return HttpContext.Current.Items[key];
		}
	}
}
