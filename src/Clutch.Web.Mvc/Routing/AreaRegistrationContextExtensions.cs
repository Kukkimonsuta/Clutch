using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;

namespace Clutch.Web.Mvc.Routing
{
	public static class AreaRegistrationContextExtensions
	{
		public static Route MapComplexRoute(this AreaRegistrationContext context, string name, string url, object rules)
		{
			return context.MapComplexRoute(name, url, rules, null, null);
		}

		public static Route MapComplexRoute(this AreaRegistrationContext context, string name, string url, object rules, object defaults)
		{
			return context.MapComplexRoute(name, url, rules, defaults, null);
		}

		public static Route MapComplexRoute(this AreaRegistrationContext context, string name, string url, object rules, string[] namespaces)
		{
			return context.MapComplexRoute(name, url, rules, null, null, namespaces);
		}

		public static Route MapComplexRoute(this AreaRegistrationContext context, string name, string url, object rules, object defaults, object constraints)
		{
			return context.MapComplexRoute(name, url, rules, defaults, constraints, null);
		}

		public static Route MapComplexRoute(this AreaRegistrationContext context, string name, string url, object rules, object defaults, string[] namespaces)
		{
			return context.MapComplexRoute(name, url, rules, defaults, null, namespaces);
		}

		public static Route MapComplexRoute(this AreaRegistrationContext context, string name, string url, object rules, object defaults, object constraints, string[] namespaces)
		{
			if (context == null)
				throw new ArgumentNullException("context");
			if (url == null)
				throw new ArgumentNullException("url");
			if (rules == null)
				throw new ArgumentNullException("rules");

			if (namespaces == null && context.Namespaces != null)
				namespaces = context.Namespaces.ToArray();

			var route = context.Routes.MapComplexRoute(name, url, rules, defaults, constraints, namespaces);

			route.DataTokens["area"] = context.AreaName;
			route.DataTokens["UseNamespaceFallback"] = namespaces == null || namespaces.Length == 0;

			return route;
		}
	}
}
