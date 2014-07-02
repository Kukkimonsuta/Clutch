using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Clutch.Web.Mvc
{
	public static class TagBuilderExtensions
	{
		private static readonly string[] VoidTags = new[] 
		{ 
			"area", "base", "br", "col", "command", "embed", "hr", "img", "input", 
			"keygen", "link", "meta", "param", "source", "track", "wbr"
		};

		/// <summary>
		/// Format html tag as self-closing for void tags, normally otherwise.
		/// </summary>
		public static string ToStringAuto(this TagBuilder builder)
		{
			if (builder == null)
				throw new ArgumentNullException("builder");

			if (VoidTags.Contains(builder.TagName, StringComparer.OrdinalIgnoreCase))
			{
				if (!string.IsNullOrEmpty(builder.InnerHtml))
					throw new InvalidOperationException("Void tags cannot have inner html");

				return builder.ToString(TagRenderMode.SelfClosing);
			}
			else
				return builder.ToString(TagRenderMode.Normal);
		}
	}
}
