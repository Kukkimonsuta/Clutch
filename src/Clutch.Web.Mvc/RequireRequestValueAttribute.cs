using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Mvc;

namespace Clutch.Web.Mvc
{
	/// <summary>
	/// Selects associated action only when givet attribute is present
	/// </summary>
	public class RequireRequestValueAttribute : ActionMethodSelectorAttribute
	{
		public RequireRequestValueAttribute(string name)
		{
			if (name == null)
				throw new ArgumentNullException("name");

			Name = name;
		}
		public RequireRequestValueAttribute(string name, string value)
		{
			if (name == null)
				throw new ArgumentNullException("name");

			Name = name;
			Value = value;
		}

		public string Name { get; private set; }
		public string Value { get; set; }

		public override bool IsValidForRequest(ControllerContext controllerContext, MethodInfo methodInfo)
		{
			var actual = controllerContext.HttpContext.Request[Name];
			if (string.IsNullOrEmpty(actual) && string.IsNullOrEmpty(Value))
				return true;

			return string.Equals(actual, Value, StringComparison.OrdinalIgnoreCase);
		}
	}
}
