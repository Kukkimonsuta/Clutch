using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace Clutch.Web.Mvc
{
	public static class HtmlHelperExtensions
	{
		/// <summary>
		/// Renders partial view passing parameters into ViewData
		/// </summary>
		public static void RenderPartial(this HtmlHelper helper, string partialViewName, object model, object parameters)
		{
			var viewData = new ViewDataDictionary(helper.ViewData);
			if (parameters != null)
			{
				foreach (var pair in new RouteValueDictionary(parameters))
					viewData[pair.Key] = pair.Value;
			}

			RenderPartialExtensions.RenderPartial(helper, partialViewName, model, viewData);
		}

		/// <summary>
		/// Renders partial view using an expression as prefix
		/// </summary>
		public static void RenderPartialFor<TModel, TProperty>(this HtmlHelper<TModel> helper, string partialViewName, Expression<Func<TModel, TProperty>> expression, object parameters = null)
		{
			if (helper == null)
				throw new ArgumentNullException("helper");

			var expressionText = ExpressionHelper.GetExpressionText(expression);
			var model = helper.ViewData.GetViewDataInfo(expressionText);

			var originalPrefix = helper.ViewData.TemplateInfo.HtmlFieldPrefix;
			try
			{
				var viewData = new ViewDataDictionary(helper.ViewData);
				viewData.TemplateInfo.HtmlFieldPrefix = (string.IsNullOrEmpty(originalPrefix) ? "" : originalPrefix + ".") + expressionText;
				if (parameters != null)
				{
					foreach (var pair in new RouteValueDictionary(parameters))
						viewData[pair.Key] = pair.Value;
				}

				RenderPartialExtensions.RenderPartial(helper, partialViewName, model.Value, viewData);
			}
			finally
			{
				helper.ViewData.TemplateInfo.HtmlFieldPrefix = originalPrefix;
			}
		}
	}
}
