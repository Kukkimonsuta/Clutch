using System;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using Clutch.Diagnostics.Logging;
using Microsoft.Web.Infrastructure.DynamicValidationHelper;

namespace Clutch.Web.Diagnostics.Logging
{
	public class RequestInterceptor : ILogEventInterceptor
	{
		private const string KEY_REQUEST_ID = "request.id";
		private const string KEY_REQUEST_IP = "request.ip";
		private const string KEY_REQUEST_URL = "request.url";
		private const string KEY_REQUEST_REFERRER = "request.referrer";
		private const string KEY_REQUEST_AGENT = "request.agent";
		private const string KEY_REQUEST_COOKIES = "request.cookies";
		private const string KEY_REQUEST_POST = "request.post";

		private RequestInterceptor()
		{ }

		#region ILogEventInterceptor

		public void Prepare(ILogEvent logEvent)
		{
			var context = HttpContext.Current;
			if (context == null)
				return;

			var request = context.Request;
			if (request == null)
				return;

			Func<NameValueCollection> formGetter;
			Func<NameValueCollection> queryStringGetter;

			ValidationUtility.GetUnvalidatedCollections(HttpContext.Current, out formGetter, out queryStringGetter);

			var form = formGetter();

			logEvent.Set(KEY_REQUEST_ID, request.GetRequestId());
			logEvent.Set(KEY_REQUEST_IP, request.GetClientAddress());
			logEvent.Set(KEY_REQUEST_URL, request.Url);
			logEvent.Set(KEY_REQUEST_REFERRER, request.UrlReferrer);
			logEvent.Set(KEY_REQUEST_AGENT, request.UserAgent);

			logEvent.Set(KEY_REQUEST_COOKIES, string.Join("; ", request.Cookies.AllKeys.Select(k => string.Format("{0} = '{1}'", k, request.Cookies[k].Value))));
			logEvent.Set(KEY_REQUEST_POST, string.Join("; ", form.AllKeys.Select(k => string.Format("{0} = '{1}'", k, form[k]))));
		}

		public void Render(ILogEvent logEvent, XElement message)
		{
			var requestElement = message.Element("request");
			if (requestElement == null)
			{
				requestElement = new XElement("request");
				message.Add(requestElement);
			}
			requestElement.SetAttributeValue("id", logEvent.TryGet(KEY_REQUEST_ID, null));
			requestElement.SetAttributeValue("ip", logEvent.TryGet(KEY_REQUEST_IP, null));
			requestElement.SetAttributeValue("url", logEvent.TryGet(KEY_REQUEST_URL, null));
			requestElement.SetAttributeValue("referrer", logEvent.TryGet(KEY_REQUEST_REFERRER, null));

			if (logEvent.IsSet(KEY_REQUEST_AGENT))
			{
				var agentElement = requestElement.Element("agent");
				if (agentElement == null)
				{
					agentElement = new XElement("agent");
					requestElement.Add(agentElement);
				}
				agentElement.SetValue(logEvent.Get(KEY_REQUEST_AGENT));
			}

			if (logEvent.IsSet(KEY_REQUEST_COOKIES))
			{
				var cookiesElement = requestElement.Element("cookies");
				if (cookiesElement == null)
				{
					cookiesElement = new XElement("cookies");
					requestElement.Add(cookiesElement);
				}
				cookiesElement.SetValue(logEvent.Get(KEY_REQUEST_COOKIES));
			}

			if (logEvent.IsSet(KEY_REQUEST_POST))
			{
				var postElement = requestElement.Element("post");
				if (postElement == null)
				{
					postElement = new XElement("post");
					requestElement.Add(postElement);
				}
				postElement.SetValue(logEvent.Get(KEY_REQUEST_POST));
			}
		}

		#endregion

		#region Static members

		public static readonly RequestInterceptor Instance = new RequestInterceptor();

		#endregion
	}
}
