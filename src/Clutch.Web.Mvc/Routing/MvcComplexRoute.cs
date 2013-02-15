using Clutch.Web.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;

namespace Clutch.Web.Mvc.Routing
{
    public class MvcComplexRoute : ComplexRoute
    {
        public MvcComplexRoute(string url, RouteValueDictionary rules, IRouteHandler routeHandler)
            : base(url, rules, routeHandler)
        { }
        public MvcComplexRoute(string url, RouteValueDictionary rules, RouteValueDictionary defaults, IRouteHandler routeHandler)
            : base(url, rules, defaults, routeHandler)
        { }
        public MvcComplexRoute(string url, RouteValueDictionary rules, RouteValueDictionary defaults, RouteValueDictionary constraints, IRouteHandler routeHandler)
            : base(url, rules, defaults, constraints, routeHandler)
        { }
        public MvcComplexRoute(string url, RouteValueDictionary rules, RouteValueDictionary defaults, RouteValueDictionary constraints, RouteValueDictionary dataTokens, IRouteHandler routeHandler)
            : base(url, rules, defaults, constraints, dataTokens, routeHandler)
        { }

        protected override bool ValueRepresentsOptionalArgument(object value)
        {
            return value == System.Web.Mvc.UrlParameter.Optional;
        }
    }
}
