using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Clutch
{
	public static class StringExtensions
	{
		private static readonly Regex DoubleUnderscores = new Regex("__+", RegexOptions.Compiled);

		/// <summary>
		/// Replaces invalid file name characters with underscores
		/// </summary>
		/// <param name="text">Input text</param>
		/// <returns>File name safe string</returns>
		public static string ClearNonSafeFileSystem(this string text)
		{
			if (text == null)
				throw new ArgumentNullException("text");
			
			// replace invalid path characters with underscores
			foreach (var ch in Path.GetInvalidFileNameChars())
				text = text.Replace(ch, '_');

			// merge multiple dashes to one
			text = DoubleUnderscores.Replace(text, "_");

			return text;
		}
	}
}
