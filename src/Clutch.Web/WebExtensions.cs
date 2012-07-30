using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Net;

namespace Clutch.Web
{
	public static class WebExtensions
	{
		#region GetClientAddress

		/// <summary>
		/// Returns client address with check for proxy
		/// </summary>
		public static string GetClientAddress(this HttpRequest request)
		{
			return GetClientAddress(new HttpRequestWrapper(request));
		}

		/// <summary>
		/// Returns client address with check for proxy
		/// </summary>
		public static string GetClientAddress(this HttpRequestBase request)
		{
			// Check for proxy/loadbalancer
			var httpForwardedFor = request.ServerVariables["HTTP_X_FORWARDED_FOR"];
			if (!string.IsNullOrWhiteSpace(httpForwardedFor))
			{
				// HTTP_X_FORWARDED_FOR can contain multiple addresses, the first should be client ip
				var addresses = httpForwardedFor.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

				if (addresses.Length > 0)
				{
					// find first valid ip address (there should be no invalid adresses, but just to be sure..)
					foreach (var ipString in addresses.Select(a => a.Trim()))
					{
						// make sure this is valid ip address
						IPAddress address;
						if (IPAddress.TryParse(ipString, out address))
							return ipString;
					}
				}
			}

			// Return standard address
			return request.ServerVariables["REMOTE_ADDR"];
		}

		#endregion

		#region GetRequestId

		private const string KEY_REQUEST_ID = "clutch.web.webextensions:requestid";

		/// <summary>
		/// Returns unique request id
		/// </summary>
		public static string GetRequestId(this HttpRequest request)
		{
			return GetRequestId(new HttpRequestWrapper(request));
		}

		/// <summary>
		/// Returns unique request id
		/// </summary>
		public static string GetRequestId(this HttpRequestBase request)
		{ 
			var httpContext = request.RequestContext.HttpContext;

			var requestId = httpContext.Items[KEY_REQUEST_ID] as string;
			if (requestId == null)
				httpContext.Items[KEY_REQUEST_ID] = requestId = Guid.NewGuid().ToString();

			return requestId;
		}

		#endregion
	}
}
