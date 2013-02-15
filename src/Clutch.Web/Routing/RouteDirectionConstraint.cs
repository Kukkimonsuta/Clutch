using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Routing;

namespace Clutch.Web.Routing
{
    public class RouteDirectionConstraint : IRouteConstraint
    {
        public RouteDirectionConstraint(RouteDirection direction)
        {
            this.direction = direction;
        }

        private RouteDirection direction;

        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            return this.direction == routeDirection;
        }
    }
}
