using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

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
			var address = request.ServerVariables["HTTP_X_FORWARDED_FOR"];
			if (!string.IsNullOrWhiteSpace(address))
			{
				// HTTP_X_FORWARDED_FOR can contain multiple addresses, the first should be client ip
				var addresses = address.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

				if (address.Length > 0)
				{
					address = addresses.First();

					return address;
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
