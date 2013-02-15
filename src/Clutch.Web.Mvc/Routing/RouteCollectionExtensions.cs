using Clutch.Web.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;

namespace Clutch.Web.Mvc.Routing
{
    public static class RouteCollectionExtensions
    {
        public static Route MapComplexRoute(this RouteCollection routes, string name, string url, object rules)
        {
            return routes.MapComplexRoute(name, url, rules, null, null);
        }

        public static Route MapComplexRoute(this RouteCollection routes, string name, string url, object rules, object defaults)
        {
            return routes.MapComplexRoute(name, url, rules, defaults, null);
        }

        public static Route MapComplexRoute(this RouteCollection routes, string name, string url, object rules, string[] namespaces)
        {
            return routes.MapComplexRoute(name, url, rules, null, null, namespaces);
        }

        public static Route MapComplexRoute(this RouteCollection routes, string name, string url, object rules, object defaults, object constraints)
        {
            return routes.MapComplexRoute(name, url, rules, defaults, constraints, null);
        }

        public static Route MapComplexRoute(this RouteCollection routes, string name, string url, object rules, object defaults, string[] namespaces)
        {
            return routes.MapComplexRoute(name, url, rules, defaults, null, namespaces);
        }

        public static Route MapComplexRoute(this RouteCollection routes, string name, string url, object rules, object defaults, object constraints, string[] namespaces)
        {
            if (routes == null)
                throw new ArgumentNullException("routes");
            if (url == null)
                throw new ArgumentNullException("url");
            if (rules == null)
                throw new ArgumentNullException("rules");

            var route = new MvcComplexRoute(url, new RouteValueDictionary(rules), new MvcRouteHandler())
            {
                Defaults = new RouteValueDictionary(defaults),
                Constraints = new RouteValueDictionary(constraints),
                DataTokens = new RouteValueDictionary()
            };

            if ((namespaces != null) && (namespaces.Length > 0))
            {
                route.DataTokens["Namespaces"] = namespaces;
            }
            routes.Add(name, route);

            return route;
        }
    }
}
