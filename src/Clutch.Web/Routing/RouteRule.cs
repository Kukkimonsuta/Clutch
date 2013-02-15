using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;

namespace Clutch.Web.Routing
{
    public abstract class RouteRule
    {
        public RouteRule(int order)
        {
            Order = order;
        }

        public abstract object ProcessIncoming(ComplexRoute route, RouteData routeData, string key, object value, RouteValueDictionary routeValues);
        public abstract object ProcessOutgoing(ComplexRoute route, RouteData routeData, string key, object value, RouteValueDictionary routeValues);

        public int Order
        {
            get;
            private set;
        }
    }
}
