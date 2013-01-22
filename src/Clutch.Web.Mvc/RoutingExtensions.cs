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
        /// Matches url against routes
        /// </summary>
        /// <remarks>
        /// This method is unsafe when using constraits that depend on headers or other values from context/request
        /// </remarks>
        /// <param name="routeCollection">Route collection</param>
        /// <param name="uriString">Uri string</param>
        /// <param name="method">Http method</param>
        /// <returns>RouteData or null</returns>
        public static RouteData GetRouteData(this RouteCollection routeCollection, string uriString, string method = "GET")
        {
            if (routeCollection == null)
                throw new ArgumentNullException("routeCollection");
            if (uriString == null)
                throw new ArgumentNullException("uriString");
            if (method == null)
                throw new ArgumentNullException("method");

            return GetRouteData(routeCollection, new Uri(uriString), method: method);
        }
        /// <summary>
        /// Matches uri against routes
        /// </summary>
        /// <remarks>
        /// This method is unsafe when using constraits that depend on headers or other values from context/request
        /// </remarks>
        /// <param name="routeCollection">Route collection</param>
        /// <param name="uri">Uri</param>
        /// <param name="method">Http method</param>
        /// <returns>RouteData or null</returns>
        public static RouteData GetRouteData(this RouteCollection routeCollection, Uri uri, string method = "GET")
        {
            if (routeCollection == null)
                throw new ArgumentNullException("routeCollection");
            if (uri == null)
                throw new ArgumentNullException("uri");
            if (method == null)
                throw new ArgumentNullException("method");

            var httpContext = CreateFakeHttpContext(uri, method: method);

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

            var requestContext = CreateFakeRequestContext(new Uri("/"));

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

            var requestContext = CreateFakeRequestContext(new Uri("/"));

            return routeCollection.GetVirtualPath(requestContext, name, values);
        }

        internal static RequestContext CreateFakeRequestContext(Uri uri, string method = "GET")
        {
            if (uri == null)
                throw new ArgumentNullException("uri");
            if (method == null)
                throw new ArgumentNullException("method");

            var httpContext = CreateFakeHttpContext(uri, method: method);

            return new RequestContext(httpContext, new RouteData());
        }

        internal static FakeHttpContext CreateFakeHttpContext(Uri uri, string method = "GET")
        {
            if (uri == null)
                throw new ArgumentNullException("uri");
            if (method == null)
                throw new ArgumentNullException("method");

            var httpRequest = CreateFakeHttpRequest(uri, method: method);

            return new FakeHttpContext(httpRequest);
        }

        internal static FakeHttpRequest CreateFakeHttpRequest(Uri uri, string method = "GET")
        {
            if (uri == null)
                throw new ArgumentNullException("uri");
            if (method == null)
                throw new ArgumentNullException("method");

            return new FakeHttpRequest(uri, method: method);
        }

        #region Nested type: FakeHttpContext

        internal class FakeHttpContext : HttpContextBase
        {
            public FakeHttpContext(HttpRequestBase request)
            {
                this.request = request;
            }

            private HttpRequestBase request;

            public override HttpRequestBase Request
            {
                get
                {
                    return request;
                }
            }
        }

        #endregion

        #region Nested type: FakeHttpRequest

        internal class FakeHttpRequest : HttpRequestBase
        {
            public FakeHttpRequest(Uri uri, string method = "GET")
            {
                this.uri = uri;
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
