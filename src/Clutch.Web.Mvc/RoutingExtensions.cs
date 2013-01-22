using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Routing;

namespace Clutch.Web.Mvc
{
    public static class RoutingExtensions
    {
        /// <summary>
        /// Matches uri against routes
        /// </summary>
        /// <remarks>
        /// This method is unsafe when using constraits that depend on headers or other values from context/request
        /// </remarks>
        /// <param name="routeCollection">Route collection</param>
        /// <param name="absolutePathAndQuery">Absolute path and query</param>
        /// <param name="method">Http method</param>
        /// <returns>RouteData or null</returns>
        public static RouteData GetRouteData(this RouteCollection routeCollection, string absolutePathAndQuery, string method = "GET")
        {
            if (routeCollection == null)
                throw new ArgumentNullException("routeCollection");
            if (absolutePathAndQuery == null)
                throw new ArgumentNullException("absolutePathAndQuery");
            if (method == null)
                throw new ArgumentNullException("method");

            var httpContext = CreateFakeHttpContext(absolutePathAndQuery, method: method);

            return routeCollection.GetRouteData(httpContext);
        }

        /// <summary>
        /// Constructs url from route values
        /// </summary>
        /// <param name="routeCollection">Route collection</param>
        /// <param name="values">Route values</param>
        /// <returns>VirtualPathData or null</returns>
        public static VirtualPathData GetVirtualPath(this RouteCollection routeCollection, RouteValueDictionary values)
        {
            if (routeCollection == null)
                throw new ArgumentNullException("routeCollection");

            var requestContext = CreateFakeRequestContext("/");

            return routeCollection.GetVirtualPath(requestContext, values);
        }
        /// <summary>
        /// Constructs url from route values using named route
        /// </summary>
        /// <param name="routeCollection">Route collection</param>
        /// <param name="name">Route name</param>
        /// <param name="values">Route values</param>
        /// <returns>VirtualPathData or null</returns>
        public static VirtualPathData GetVirtualPath(this RouteCollection routeCollection, string name, RouteValueDictionary values)
        {
            if (routeCollection == null)
                throw new ArgumentNullException("routeCollection");

            var requestContext = CreateFakeRequestContext("/");

            return routeCollection.GetVirtualPath(requestContext, name, values);
        }

        internal static RequestContext CreateFakeRequestContext(string absolutePathAndQuery, string method = "GET")
        {
            if (absolutePathAndQuery == null)
                throw new ArgumentNullException("absolutePathAndQuery");
            if (method == null)
                throw new ArgumentNullException("method");

            var httpContext = CreateFakeHttpContext(absolutePathAndQuery, method: method);

            return new RequestContext(httpContext, new RouteData());
        }

        internal static FakeHttpContext CreateFakeHttpContext(string absolutePathAndQuery, string method = "GET")
        {
            if (absolutePathAndQuery == null)
                throw new ArgumentNullException("absolutePathAndQuery");
            if (method == null)
                throw new ArgumentNullException("method");

            var httpRequest = CreateFakeHttpRequest(absolutePathAndQuery, method: method);

            return new FakeHttpContext(httpRequest);
        }

        internal static FakeHttpRequest CreateFakeHttpRequest(string absolutePathAndQuery, string method = "GET")
        {
            if (absolutePathAndQuery == null)
                throw new ArgumentNullException("absolutePathAndQuery");
            if (method == null)
                throw new ArgumentNullException("method");

            return new FakeHttpRequest(absolutePathAndQuery, method: method);
        }

        #region Nested type: FakeHttpContext

        internal class FakeHttpContext : HttpContextBase
        {
            public FakeHttpContext(HttpRequestBase request)
            {
                this.request = request;
            }

            private HttpRequestBase request;
            private HttpResponseBase response;

            public override HttpRequestBase Request
            {
                get
                {
                    return request;
                }
            }

            public override HttpResponseBase Response
            {
                get
                {
                    if (response == null)
                        response = new FakeHttpResponse();

                    return response;
                }
            }
        }

        #endregion

        #region Nested type: FakeHttpResponse

        internal class FakeHttpResponse : HttpResponseBase
        {
            public FakeHttpResponse()
            { 
            }

            public override string ApplyAppPathModifier(string virtualPath)
            {
                // this should be handled better probably, for now this is sufficient
                return HttpContext.Current.Response.ApplyAppPathModifier(virtualPath);
            }
        }

        #endregion

        #region Nested type: FakeHttpRequest

        internal class FakeHttpRequest : HttpRequestBase
        {
            public FakeHttpRequest(string absolutePathAndQuery, string method = "GET")
            {
                if (absolutePathAndQuery == null)
                    throw new ArgumentNullException("absolutePathAndQuery");
                if (method == null)
                    throw new ArgumentNullException("method");

                if (!absolutePathAndQuery.StartsWith("/"))
                    absolutePathAndQuery = "/" + absolutePathAndQuery;

                this.uri = new Uri("http://localhost" + absolutePathAndQuery);
                this.method = method.ToUpperInvariant();
            }

            private Uri uri;
            private string method;
            private string applicationPath;
            private string path;
            private string appRelativeCurrentExecutionFilePath;

            public override string HttpMethod
            {
                get
                {
                    return base.HttpMethod;
                }
            }

            public override string RawUrl
            {
                get
                {
                    return uri.PathAndQuery;
                }
            }

            public override string ApplicationPath
            {
                get
                {
                    if (applicationPath == null)
                        applicationPath = VirtualPathUtility.ToAbsolute("~/");

                    return applicationPath;
                }
            }

            public override string PathInfo
            {
                get
                {
                    // isn't used in mvc projects
                    return "";
                }
            }

            public override string Path
            {
                get
                {
                    if (path == null)
                    {
                        path = uri.AbsolutePath;
                    }

                    return path;
                }
            }

            public override string FilePath
            {
                get
                {
                    // in mvc projects is always the same as Path
                    return Path;
                }
            }

            public override string CurrentExecutionFilePath
            {
                get
                {
                    // in mvc projects is always the same as Path
                    return Path;
                }
            }

            public override string AppRelativeCurrentExecutionFilePath
            {
                get
                {
                    if (appRelativeCurrentExecutionFilePath == null)
                    {
                        appRelativeCurrentExecutionFilePath = "~" + CurrentExecutionFilePath.Substring(ApplicationPath.Length - 1);
                    }

                    return appRelativeCurrentExecutionFilePath;
                }
            }
        }

        #endregion
    }
}
