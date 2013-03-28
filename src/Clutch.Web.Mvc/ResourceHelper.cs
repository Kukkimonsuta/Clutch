using System;
using System.Web;
using System.Web.Mvc;
using Clutch.Infrastructure;

namespace Clutch.Web.Mvc
{
    public static class ResourcetHelper
    {
        #region Resource

        /// <summary>
        /// Returns url for the requested resource
        /// </summary>
        /// <param name="context">Relevant http context</param>
        /// <param name="folder">Folder name</param>
        /// <param name="name">Resource name without extension</param>
        /// <param name="extension">Resource extension</param>
        /// <param name="minExtension">Minified resource extension</param>
        /// <param name="ignoreMinified">Don't serve minified resource</param>
        /// <param name="includeRevision">Include application assembly revision in url to disable cache</param>
        /// <returns>Url to requested resource</returns>
        public static string Resource(HttpContextBase context, string folder, string name, string extension, string minExtension, bool ignoreMinified, bool includeRevision)
        {
            if (folder == null)
                throw new ArgumentNullException("folder");
            if (name == null)
                throw new ArgumentNullException("name");
            if (context == null)
                throw new ArgumentNullException("context");

            var applicationConfigurator = ServiceLocator.GetInstance<IApplicationConfigurator>();
            var revision = applicationConfigurator.Assembly.GetName().Version.Revision;

            if (applicationConfigurator.IsDebug)
                ignoreMinified = true;

            folder = folder.Trim('/', '~') + "/";
            name = name.TrimStart('/');

            var path = string.Format("~/{0}{1}{2}{3}", folder, name,
                ignoreMinified ? extension : minExtension,
                includeRevision ? "?_=" + revision : string.Empty
            );

            return UrlHelper.GenerateContentUrl(path, context);
        }

        #endregion

        #region Script

        public static string ScriptsFolder = "Scripts";

        /// <summary>
        /// Returns url to requested script
        /// </summary>
        /// <param name="context">Relevant http context</param>
        /// <param name="scriptName">Script name</param>
        /// <param name="ignoreMinified">Don't serve minified script</param>
        /// <param name="includeRevision">Include application assembly revision in url to disable cache</param>
        /// <returns>Url to requested script</returns>
        public static string Script(HttpContextBase context, string scriptName, string suffix = null, bool ignoreMinified = false, bool includeRevision = true)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            if (scriptName == null)
                throw new ArgumentNullException("scriptName");

            return Resource(context, ScriptsFolder, scriptName, suffix == null ? ".js" : suffix + ".js", ".min.js", ignoreMinified, includeRevision);
        }

        /// <summary>
        /// Returns url to requested script
        /// </summary>
        /// <param name="self">Url helper</param>
        /// <param name="scriptName">Script name</param>
        /// <param name="ignoreMinified">Don't serve minified script</param>
        /// <returns>Url to requested script</returns>
        public static string Script(this UrlHelper urlHelper, string scriptName, string suffix = null, bool ignoreMinified = false, bool includeRevision = true)
        {
            if (urlHelper == null)
                throw new ArgumentNullException("urlHelper");
            if (scriptName == null)
                throw new ArgumentNullException("scriptName");

            return Script(urlHelper.RequestContext.HttpContext, scriptName, suffix: suffix, ignoreMinified: ignoreMinified, includeRevision: includeRevision);
        }

        /// <summary>
        /// Returns script element with url for requested script
        /// </summary>
        /// <param name="self">Html helper</param>
        /// <param name="scriptName">Script name</param>
        /// <param name="ignoreMinified">Don't serve minified script</param>
        /// <returns>Script element</returns>
        public static MvcHtmlString Script(this HtmlHelper htmlHelper, string scriptName, string suffix = null, bool ignoreMinified = false, bool includeRevision = true)
        {
            if (htmlHelper == null)
                throw new ArgumentNullException("htmlHelper");
            if (scriptName == null)
                throw new ArgumentNullException("scriptName");

            var scriptUrl = Script(htmlHelper.ViewContext.HttpContext, scriptName, suffix: suffix, ignoreMinified: ignoreMinified, includeRevision: includeRevision);

            var scriptTag = new TagBuilder("script");
            scriptTag.Attributes.Add("src", scriptUrl);
            scriptTag.Attributes.Add("type", "text/javascript");

            return MvcHtmlString.Create(scriptTag.ToString(TagRenderMode.Normal));
        }

        #endregion

        #region Styles

        public static string StylesFolder = "Content";

        /// <summary>
        /// Returns url to requested style
        /// </summary>
        /// <param name="context">Relevant http context</param>
        /// <param name="styleName">Style name</param>
        /// <param name="ignoreMinified">Don't serve minified style</param>
        /// <param name="includeRevision">Include application assembly revision in url to disable cache</param>
        /// <returns>Url to requested style</returns>
        public static string Style(HttpContextBase context, string styleName, string suffix = null, bool ignoreMinified = false, bool includeRevision = true)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            if (styleName == null)
                throw new ArgumentNullException("styleName");

            return Resource(context, StylesFolder, styleName, suffix == null ? ".css" : suffix + ".css", ".min.css", ignoreMinified, includeRevision);
        }

        /// <summary>
        /// Returns url to requested style
        /// </summary>
        /// <param name="self">Url helper</param>
        /// <param name="styleName">Style name</param>
        /// <param name="ignoreMinified">Don't serve minified style</param>
        /// <returns>Url to requested style</returns>
        public static string Style(this UrlHelper urlHelper, string styleName, string suffix = null, bool ignoreMinified = false, bool includeRevision = true)
        {
            if (urlHelper == null)
                throw new ArgumentNullException("urlHelper");
            if (styleName == null)
                throw new ArgumentNullException("styleName");

            return Style(urlHelper.RequestContext.HttpContext, styleName, suffix: suffix, ignoreMinified: ignoreMinified, includeRevision: includeRevision);
        }

        /// <summary>
        /// Returns style element with url for requested style
        /// </summary>
        /// <param name="self">Html helper</param>
        /// <param name="styleName">Style name</param>
        /// <param name="ignoreMinified">Don't serve minified style</param>
        /// <returns>Style element</returns>
        public static MvcHtmlString Style(this HtmlHelper htmlHelper, string styleName, string suffix = null, bool ignoreMinified = false, bool includeRevision = true, string media = null)
        {
            if (htmlHelper == null)
                throw new ArgumentNullException("htmlHelper");
            if (styleName == null)
                throw new ArgumentNullException("styleName");

            var styleUrl = Style(htmlHelper.ViewContext.HttpContext, styleName, suffix: suffix, ignoreMinified: ignoreMinified, includeRevision: includeRevision);
            
            var linkTag = new TagBuilder("link");

            linkTag.Attributes.Add("href", styleUrl);
            linkTag.Attributes.Add("rel", "stylesheet");
            linkTag.Attributes.Add("type", "text/css");
            if (media != null)
                linkTag.Attributes.Add("media", media);

            return MvcHtmlString.Create(linkTag.ToString(TagRenderMode.SelfClosing));
        }

        #endregion
    }
}
