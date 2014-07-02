using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Clutch.Web.Mvc
{
	/// <summary>
	/// Require authorization for specified areas.
	/// </summary>
	public class AreaAuthorizeAttribute : AuthorizeAttribute
	{
		public AreaAuthorizeAttribute()
		{ }
		public AreaAuthorizeAttribute(string areas)
		{
			Areas = areas;
		}

		private string m_areas;
		private string[] m_areaList;

		/// <summary>
		/// Limit this attribute only for given areas.
		/// </summary>
		public string Areas
		{
			get { return m_areas; }
			set
			{
				m_areas = value;

				if (value != null)
				{
					m_areaList = value.Split(new[] { ',', ' ', ';' }, StringSplitOptions.RemoveEmptyEntries)
						.Select(a => a.Trim().ToLower())
						.Where(a => a.Length > 0)
						.ToArray();
				}
				else
					m_areaList = new string[0];
			}
		}

		protected override bool AuthorizeCore(HttpContextBase httpContext)
		{
			var request = httpContext.Request;
			var area = request.RequestContext.RouteData.DataTokens["area"] as string ?? string.Empty;

			// we are not inside an area, ignore this attribute
			if (string.IsNullOrEmpty(area))
				return true;

			// no specific area, all areas require authorization
			if (m_areaList.Length <= 0)
				return base.AuthorizeCore(httpContext);

			// current area is not within filter, ignore this attribute
			if (!m_areaList.Contains(area.ToLower()))
				return true;

			// this area requires authorization
			return base.AuthorizeCore(httpContext);
		}
	}
}
