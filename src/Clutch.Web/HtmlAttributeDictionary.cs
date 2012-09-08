using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;

namespace Clutch.Web
{
	/// <summary>
	/// Dictionary with functionality to replace underscores to hypens in keys
	/// </summary>
	public class HtmlAttributeDictionary : RouteValueDictionary
	{
		public HtmlAttributeDictionary()
			: base()
		{ }
		public HtmlAttributeDictionary(object values)
			: base(values)
		{
			ReplaceUnderscores();
		}
		public HtmlAttributeDictionary(IDictionary<string, object> dictionary)
			: base(dictionary)
		{
			ReplaceUnderscores();
		}

		public void ReplaceUnderscores()
		{
			var pairsToReplace = this.Where(p => p.Key.Contains("_")).ToArray();

			foreach (var pair in pairsToReplace)
			{
				this.Remove(pair.Key);
				this.Add(pair.Key.Replace("_", "-"), pair.Value);
			}
		}
	}
}
