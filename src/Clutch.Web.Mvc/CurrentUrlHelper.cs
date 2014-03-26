using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using System.Text.RegularExpressions;
using System.Web;

namespace Clutch.Web.Mvc
{
    public static class CurrentUrlHelper
    {
        private static readonly string[] IgnoreValues = new string[] { 
			"_" /* used by ajax requests to ignore cache */
		};

        private static Regex IsGuid = new Regex("^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        #region Current

        /// <summary>
        /// Returns current url modified by parameters
        /// </summary>
        /// <param name="context">Relevant request context</param>
        /// <param name="with">Values to add or override</param>
        /// <param name="without">Values to remove if present</param>
        /// <returns>Route values</returns>
        public static RouteValueDictionary Current(RequestContext context, object with = null, string[] without = null)
        {
            return Current(context, new RouteValueDictionary(with), without);
        }

        /// <summary>
        /// Returns current url modified by parameters
        /// </summary>
        /// <remarks>
        /// Removes keyless query string values
        /// </remarks>
        /// <param name="context">Relevant request context</param>
        /// <param name="with">Values to add or override</param>
        /// <param name="without">Values to remove if present</param>
        /// <returns>Route values</returns>
        public static RouteValueDictionary Current(RequestContext context, RouteValueDictionary with, string[] without)
        {
            with = with ?? new RouteValueDictionary();
            without = without ?? new string[0];

            var result = new RouteValueDictionary();

            var queryString = context.HttpContext.Request.QueryString;
            foreach (var key in queryString.AllKeys.Where(k => k != null && !IgnoreValues.Contains(k) && !IsGuid.IsMatch(k) && !without.Contains(k)))
                result[key] = queryString[key];

            var routeValues = context.RouteData.Values;
            foreach (var pair in routeValues.Where(p => !IgnoreValues.Contains(p.Key) && !IsGuid.IsMatch(p.Key) && !without.Contains(p.Key)))
                result[pair.Key] = pair.Value;

			var area = context.RouteData.DataTokens["area"];
			if (area != null && !without.Contains("area"))
				result["area"] = area;

            foreach (var pair in with.Where(p => !IgnoreValues.Contains(p.Key) && !IsGuid.IsMatch(p.Key) && !without.Contains(p.Key)))
                result[pair.Key] = pair.Value;

            return result;
        }

        /// <summary>
        /// Returns current url of parent context modified by parameters
        /// </summary>
        /// <param name="context">Parent request context</param>
        /// <param name="with">Values to add or override</param>
        /// <param name="without">Values to remove if present</param>
        /// <returns>Route values</returns>
        public static RouteValueDictionary ParentCurrent(RequestContext context, object with = null, string[] without = null)
        {
            return ParentCurrent(context, new RouteValueDictionary(with), without);
        }

        /// <summary>
        /// Returns current url of parent context modified by parameters
        /// </summary>
        /// <param name="context">Parent request context</param>
        /// <param name="with">Values to add or override</param>
        /// <param name="without">Values to remove if present</param>
        /// <returns>Route values</returns>
        public static RouteValueDictionary ParentCurrent(RequestContext context, RouteValueDictionary with = null, string[] without = null)
        {
            var parentContext = context.RouteData.DataTokens["ParentActionViewContext"] as ViewContext;

            if (parentContext != null)
                return Current(parentContext.RequestContext, with, without);

            return Current(context, with, without);
        }

        /// <summary>
        /// Returns current url modified by parameters
        /// </summary>
        /// <param name="self">Url helper</param>
        /// <param name="with">Values to add or override</param>
        /// <param name="without">Values to remove if present</param>
        /// <returns>Url</returns>
        public static string Current(this UrlHelper self, object with = null, string[] without = null)
        {
            var routeValues = Current(self.RequestContext, with, without);

            // makes sure that "without" parameter works
            var urlHelper = new UrlHelper(RoutingExtensions.CreateFakeRequestContext("/"));

            return urlHelper.Action(null, routeValues);
        }

        /// <summary>
        /// Returns current route values modified by parameters
        /// </summary>
        /// <param name="self">Url helper</param>
        /// <param name="with">Values to add or override</param>
        /// <param name="without">Values to remove if present</param>
        /// <returns>Route values</returns>
        public static RouteValueDictionary CurrentValues(this UrlHelper self, object with = null, string[] without = null)
        {
            return Current(self.RequestContext, with, without);
        }

        /// <summary>
        /// Returns current route values of parent context modified by parameters
        /// </summary>
        /// <param name="self">Url helper</param>
        /// <param name="with">Values to add or override</param>
        /// <param name="without">Values to remove if present</param>
        /// <returns>Route values</returns>
        public static RouteValueDictionary ParentCurrentValues(this UrlHelper self, object with = null, string[] without = null)
        {
            return ParentCurrent(self.RequestContext, with, without);
        }

        /// <summary>
        /// Returns anchor element with current url modified by parameters
        /// </summary>
        /// <param name="self">Html helper</param>
        /// <param name="linkText">The inner text of anchor element</param>
        /// <param name="with">Values to add or override</param>
        /// <param name="without">Values to remove if present</param>
        /// <returns>Anchor element</returns>
        public static MvcHtmlString CurrentLink(this HtmlHelper self, string linkText, object with = null, string[] without = null)
        {
            return CurrentLink(self, linkText, null, with, without);
        }

        /// <summary>
        /// Returns anchor element with current url modified by parameters
        /// </summary>
        /// <param name="self">Html helper</param>
        /// <param name="linkText">The inner text of anchor element</param>
        /// <param name="htmlAttributes">Additional html attributes</param>
        /// <param name="with">Values to add or override</param>
        /// <param name="without">Values to remove if present</param>
        /// <returns>Anchor element</returns>
        public static MvcHtmlString CurrentLink(this HtmlHelper self, string linkText, object htmlAttributes, object with = null, string[] without = null)
        {
            var routeValues = Current(self.ViewContext.RequestContext, with, without);

            // makes sure that "without" parameter works
            var urlHelper = new UrlHelper(RoutingExtensions.CreateFakeRequestContext("/"));

            TagBuilder builder = new TagBuilder("a");
            builder.InnerHtml = !string.IsNullOrEmpty(linkText) ? HttpUtility.HtmlEncode(linkText) : string.Empty;
            builder.MergeAttributes<string, object>(new RouteValueDictionary(htmlAttributes));
            builder.MergeAttribute("href", urlHelper.Action(null, routeValues));
            return MvcHtmlString.Create(builder.ToString(TagRenderMode.Normal));
        }

        /// <summary>
        /// Returns anchor element with current url of parent context modified by parameters
        /// </summary>
        /// <param name="self">Html helper</param>
        /// <param name="linkText">The inner text of anchor element</param>
        /// <param name="with">Values to add or override</param>
        /// <param name="without">Values to remove if present</param>
        /// <returns>Anchor element</returns>
        public static MvcHtmlString ParentCurrentLink(this HtmlHelper self, string linkText, object with = null, string[] without = null)
        {
            return ParentCurrentLink(self, linkText, null, with, without);
        }

        /// <summary>
        /// Returns anchor element with current url of parent context modified by parameters
        /// </summary>
        /// <param name="self">Html helper</param>
        /// <param name="linkText">The inner text of anchor element</param>
        /// <param name="htmlAttributes">Additional html attributes</param>
        /// <param name="with">Values to add or override</param>
        /// <param name="without">Values to remove if present</param>
        /// <returns>Anchor element</returns>
        public static MvcHtmlString ParentCurrentLink(this HtmlHelper self, string linkText, object htmlAttributes, object with = null, string[] without = null)
        {
            var routeValues = ParentCurrent(self.ViewContext.RequestContext, with, without);

            // makes sure that "without" parameter works
            var urlHelper = new UrlHelper(RoutingExtensions.CreateFakeRequestContext("/"));

            TagBuilder builder = new TagBuilder("a");
            builder.InnerHtml = !string.IsNullOrEmpty(linkText) ? HttpUtility.HtmlEncode(linkText) : string.Empty;
            builder.MergeAttributes<string, object>(new RouteValueDictionary(htmlAttributes));
            builder.MergeAttribute("href", urlHelper.Action(null, routeValues));
            return MvcHtmlString.Create(builder.ToString(TagRenderMode.Normal));
        }

        #endregion

        #region Is

        /// <summary>
        /// Returns whether current url would match specified route values
        /// </summary>
        /// <param name="context">Request context</param>
        /// <param name="routeValues">Route values</param>
        /// <returns>Boolean</returns>
        public static bool Is(RequestContext context, object routeValues)
        {
            return Is(context, new RouteValueDictionary(routeValues));
        }

        /// <summary>
        /// Returns whether current url would match specified route values
        /// </summary>
        /// <param name="context">Request context</param>
        /// <param name="routeValues">Route values</param>
        /// <returns>Boolean</returns>
        public static bool Is(RequestContext context, RouteValueDictionary routeValues, bool includeParentRequest = false)
        {
            var queryString = context.HttpContext.Request.QueryString;
            var currentValues = context.RouteData.Values;
            var dataTokens = context.RouteData.DataTokens;

            foreach (var pair in routeValues)
            {
                var value = pair.Value.ToString().ToUpperInvariant();

                if (queryString[pair.Key] != null && queryString[pair.Key].ToUpperInvariant() == value)
                    continue;

                if (currentValues.ContainsKey(pair.Key) && currentValues[pair.Key].ToString().ToUpperInvariant() == value)
                    continue;

                if (dataTokens.ContainsKey(pair.Key))
                {
                    if (dataTokens[pair.Key].ToString().ToUpperInvariant() == value)
                        continue;
                }
                else if (pair.Key.ToUpperInvariant() == "AREA" && value == string.Empty)
                    continue;

                return false;
            }

            return true;
        }

        /// <summary>
        /// Returns whether parent request url would match specified route values
        /// </summary>
        /// <param name="context">Request context</param>
        /// <param name="routeValues">Route values</param>
        /// <returns>Boolean</returns>
        public static bool ParentIs(RequestContext context, object routeValues)
        {
            return ParentIs(context, new RouteValueDictionary(routeValues));
        }

        /// <summary>
        /// Returns whether parent request url would match specified route values
        /// </summary>
        /// <param name="context">Request context</param>
        /// <param name="routeValues">Route values</param>
        /// <returns>Boolean</returns>
        public static bool ParentIs(RequestContext context, RouteValueDictionary routeValues)
        {
            var parentContext = context.RouteData.DataTokens["ParentActionViewContext"] as ViewContext;

            if (parentContext != null)
                return Is(parentContext.RequestContext, routeValues);

            return false;
        }

        /// <summary>
        /// Returns whether current url would match specified route values
        /// </summary>
        /// <param name="self">Url helper</param>
        /// <param name="routeValues">Route values</param>
        /// <returns>Boolean</returns>
        public static bool Is(this UrlHelper self, object routeValues)
        {
            return Is(self.RequestContext, routeValues);
        }

        /// <summary>
        /// Returns whether parent request url would match specified route values
        /// </summary>
        /// <param name="self">Url helper</param>
        /// <param name="routeValues">Route values</param>
        /// <returns>Boolean</returns>
        public static bool ParentIs(this UrlHelper self, object routeValues)
        {
            return ParentIs(self.RequestContext, routeValues);
        }

        #endregion

        #region Has

        /// <summary>
        /// Returns whether current values contain specified keys
        /// </summary>
        /// <param name="context">Request context</param>
        /// <param name="keys">Keys</param>
        /// <returns>Boolean</returns>
        public static bool Has(RequestContext context, params string[] keys)
        {
            var queryString = context.HttpContext.Request.QueryString.AllKeys.Where(k => k != null).Select(k => k.ToUpperInvariant()).ToArray();
            var currentValues = context.RouteData.Values.Where(k => k.Key != null).Select(k => k.Key.ToUpperInvariant()).ToArray();
            var dataTokens = context.RouteData.DataTokens.Where(k => k.Key != null).Select(k => k.Key.ToUpperInvariant()).ToArray();

            foreach (var key in keys.Select(k => k.ToUpperInvariant()))
            {
                if (queryString.Contains(key))
                    continue;

                if (currentValues.Contains(key))
                    continue;

                if (dataTokens.Contains(key))
                    continue;

                return false;
            }

            return true;
        }

        /// <summary>
        /// Returns whether current values of parent context contain specified keys
        /// </summary>
        /// <param name="context">Request context</param>
        /// <param name="keys">Keys</param>
        /// <returns>Boolean</returns>
        public static bool ParentHas(RequestContext context, params string[] keys)
        {
            var parentContext = context.RouteData.DataTokens["ParentActionViewContext"] as ViewContext;

            if (parentContext != null)
                return Has(parentContext.RequestContext, keys);

            return false;
        }

        /// <summary>
        /// Returns whether current values contain specified keys
        /// </summary>
        /// <param name="url">Request context</param>
        /// <param name="keys">Keys</param>
        /// <returns>Boolean</returns>
        public static bool Has(this UrlHelper url, params string[] keys)
        {
            return Has(url.RequestContext, keys);
        }

        /// <summary>
        /// Returns whether current values of parent context contain specified keys
        /// </summary>
        /// <param name="url">Request context</param>
        /// <param name="keys">Keys</param>
        /// <returns>Boolean</returns>
        public static bool ParentHas(this UrlHelper url, params string[] keys)
        {
            return ParentHas(url.RequestContext, keys);
        }

        #endregion
    }
}
