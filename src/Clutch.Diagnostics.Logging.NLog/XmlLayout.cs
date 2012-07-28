using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog.LayoutRenderers;
using NLog.Layouts;
using System.Xml;
using System.Globalization;

namespace Clutch.Diagnostics.Logging.NLog
{
	[Layout("Xml")]
	public class XmlLayout : Layout
	{
		private const string EVENT_PREPARED = "97A9736A-D767-4C9C-A28A-5A2E522E332D";

		private void PrepareEvent(global::NLog.LogEventInfo logEvent)
		{
			if (logEvent.Properties.ContainsKey(EVENT_PREPARED))
				return;

			foreach (var interceptor in GetAllInterceptors())
				interceptor.Prepare(logEvent);

			logEvent.Properties[EVENT_PREPARED] = true;
		}

		private XElement FormatException(Exception exception, string elementName = "exception")
		{
			var element = new XElement(elementName,
				new XAttribute("message", exception.Message)
			);

			if (!string.IsNullOrWhiteSpace(exception.StackTrace))
				element.Add(new XElement("stackTrace", exception.StackTrace));

			if (exception.InnerException != null)
				element.Add(FormatException(exception.InnerException, elementName: "innerException"));

			return element;
		}

		#region SimpleLayout

		protected override string GetFormattedMessage(global::NLog.LogEventInfo logEvent)
		{
			PrepareEvent(logEvent);

			var builder = new StringBuilder();

			using (var writer = XmlWriter.Create(builder, new XmlWriterSettings()
			{
				OmitXmlDeclaration = true,
				Encoding = Encoding.UTF8,
				Indent = true,
				IndentChars = "\t"
			}))
			{
				var element = new XElement("message",
					new XAttribute("time", logEvent.TimeStamp.ToUniversalTime().ToString("u", CultureInfo.InvariantCulture)),
					new XAttribute("source", logEvent.LoggerName),
					new XAttribute("level", logEvent.Level),
					new XAttribute("message", logEvent.FormattedMessage)
				);

				if (logEvent.Exception != null)
					element.Add(FormatException(logEvent.Exception));

				foreach (var interceptor in GetAllInterceptors())
					interceptor.Render(logEvent, element);

				element.Save(writer);
			}

			return builder.ToString();
		}

		#endregion

		#region Static members

		static XmlLayout()
		{
			LogEventInterceptorProviders = new List<Func<IEnumerable<IXmlLogEventInterceptor>>>();
			LogEventInterceptorProviders.Add(new Func<IEnumerable<IXmlLogEventInterceptor>>(DefaultLogEventInterceptorProvider));
		}

		private static IEnumerable<IXmlLogEventInterceptor> DefaultLogEventInterceptorProvider()
		{
			yield return Interceptors.MessageIdInterceptor.Instance;
			yield return Interceptors.IdentityInterceptor.Instance;
			yield return Interceptors.RunIdInterceptor.Instance;
		}

		private static IEnumerable<IXmlLogEventInterceptor> GetAllInterceptors()
		{
			return LogEventInterceptorProviders.SelectMany(p => p() ?? Enumerable.Empty<IXmlLogEventInterceptor>());
		}

		public static List<Func<IEnumerable<IXmlLogEventInterceptor>>> LogEventInterceptorProviders;

		#endregion
	}
}
