using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Routing;

namespace Clutch.Web.Routing
{
    public class ComplexRoute : Route
    {
        private static readonly Regex RoutePlaceholderRegex = new Regex(@"\{(?<name>[a-z0-9]+)\}", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public ComplexRoute(string url, RouteValueDictionary rules, IRouteHandler routeHandler)
            : this(url, rules, null, null, null, routeHandler)
        { }
        public ComplexRoute(string url, RouteValueDictionary rules, RouteValueDictionary defaults, IRouteHandler routeHandler)
            : this(url, rules, defaults, null, null, routeHandler)
        { }
        public ComplexRoute(string url, RouteValueDictionary rules, RouteValueDictionary defaults, RouteValueDictionary constraints, IRouteHandler routeHandler)
            : this(url, rules, defaults, constraints, null, routeHandler)
        { }
        public ComplexRoute(string url, RouteValueDictionary rules, RouteValueDictionary defaults, RouteValueDictionary constraints, RouteValueDictionary dataTokens, IRouteHandler routeHandler)
            : base(url, defaults, constraints, dataTokens, routeHandler)
        {
            Rules = new Dictionary<string, RouteRule>();
            foreach (var pair in rules)
                Rules.Add(pair.Key, (RouteRule)pair.Value);

            placeholders = RoutePlaceholderRegex.Matches(url).Cast<Match>().Select(m => m.Groups["name"].Value).ToArray();
        }

        private string[] placeholders;

        public Dictionary<string, RouteRule> Rules { get; protected set; }

        protected virtual bool ValueRepresentsOptionalArgument(object value)
        {
            return false;
        }

        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            var result = base.GetRouteData(httpContext);

            if (result == null)
                return null;

            foreach (var pair in Rules.OrderBy(p => p.Value.Order))
            {
                var currentValue = result.Values[pair.Key];
                if (currentValue == null)
                    continue;

                if (ValueRepresentsOptionalArgument(currentValue))
                    continue;

                var value = pair.Value.ProcessIncoming(this, result, pair.Key, currentValue, result.Values);
                if (value == null)
                    return null;

                result.Values[pair.Key] = value;
            }

            return result;
        }

        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary originalValues)
        {
            // this will be executed twice, but it is less evil than forcing multiple db queries of TokenRouteRule for requests
            // that cant be possibly valid
            if (base.GetVirtualPath(requestContext, originalValues) == null)
                return null;

            var values = new RouteValueDictionary(originalValues);

            // copy values that would be inherited from current request or defaults into our values
            foreach (var placeholder in placeholders.Where(p => !values.ContainsKey(p)))
            {
                if (requestContext.RouteData.Values.ContainsKey(placeholder))
                    values[placeholder] = requestContext.RouteData.Values[placeholder];
                else if (this.Defaults.ContainsKey(placeholder))
                {
                    var @default = this.Defaults[placeholder];

                    if (!ValueRepresentsOptionalArgument(@default))
                        values[placeholder] = @default;
                }
            }

            foreach (var pair in Rules.OrderByDescending(p => p.Value.Order))
            {
                var currentValue = values[pair.Key];
                if (currentValue == null)
                    continue;

                var value = pair.Value.ProcessOutgoing(this, requestContext.RouteData, pair.Key, currentValue, values);
                if (value == null)
                    return null;

                values[pair.Key] = value;
            }

            return base.GetVirtualPath(requestContext, values);
        }
    }
}
