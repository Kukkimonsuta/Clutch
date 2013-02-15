using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;

namespace Clutch.Web.Routing
{
    public class TokenRouteRule : RouteRule
    {
        public TokenRouteRule(TokenCollection tokens, string dependency = null, int order = 0)
            : base(order)
        {
            if (tokens == null)
                throw new ArgumentNullException("tokens");

            Tokens = tokens;
            Dependency = dependency;
        }

        public TokenCollection Tokens { get; private set; }
        public string Dependency { get; private set; }

        public override object ProcessIncoming(ComplexRoute route, RouteData routeData, string key, object value, RouteValueDictionary routeValues)
        {
            var token = value as string;
            if (token == null)
                return null;

            var dependency = Dependency == null ? null : routeValues[Dependency];

            var id = Tokens.Translate(token, dependency == null ? null : dependency.ToString());
            if (!id.HasValue)
                return null;

            return id.Value;
        }

        public override object ProcessOutgoing(ComplexRoute route, RouteData routeData, string key, object value, RouteValueDictionary routeValues)
        {
            var id = value as int?;
            if (id == null)
                return null;

            var dependency = Dependency == null ? null : routeValues[Dependency];

            // use current value if new value is not explicitly defined and route will use the old one
            // todo: verify whether still necessary
            if (dependency == null && route.Url.Contains("{" + Dependency + "}"))
                dependency = routeData.Values[Dependency];

            var token = Tokens.Translate(id.Value, dependency == null ? null : dependency.ToString());
            if (token == null)
                return null;

            return token;
        }
    }
}
