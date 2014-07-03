using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web.Routing;

namespace Clutch.Web.Mvc.Routing
{
	/// <summary>
	/// Route constraint checking Thread.CurrentThread.CurrentUICulture
	/// </summary>
	public class CultureConstraint : IRouteConstraint
	{
		public CultureConstraint(string name)
		{
			m_name = name;
		}

		private string m_name;

		public bool Match(System.Web.HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
		{
			if (routeDirection == RouteDirection.IncomingRequest)
				return true;

			var valuesCulture = values["culture"] as string;
			if (valuesCulture != null)
				return string.Equals(valuesCulture, m_name, StringComparison.OrdinalIgnoreCase);

			var culture = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;

			return string.Equals(culture, m_name, StringComparison.OrdinalIgnoreCase);
		}
	}
}
