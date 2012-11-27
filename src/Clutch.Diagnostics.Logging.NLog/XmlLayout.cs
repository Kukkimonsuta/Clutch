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
using System.Diagnostics;

namespace Clutch.Diagnostics.Logging.NLog
{
	[Layout("ClutchXml")]
	public class XmlLayout : Layout
	{
		private const string EVENT_PREPARED = "97A9736A-D767-4C9C-A28A-5A2E522E332D";

		private void PrepareEvent(ILogEvent logEvent)
		{
			if (logEvent.IsSet(EVENT_PREPARED))
				return;

			foreach (var interceptor in LogEventInterceptorProviders.Providers.GetInterceptors())
				interceptor.Prepare(logEvent);

			logEvent.Set(EVENT_PREPARED, true);
		}

		private XElement FormatException(Exception exception, string elementName = "exception")
		{
			var element = new XElement(elementName,
				new XAttribute("type", exception.GetType().FullName),
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
			var wrappedEvent = new LogEventWrapper(logEvent);

			PrepareEvent(wrappedEvent);

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

				object extendedInfo;
				if (logEvent.Properties.TryGetValue("extendedInfo", out extendedInfo) && extendedInfo != null)
					element.Add(new XElement("extendedInfo", extendedInfo));

				foreach (var interceptor in LogEventInterceptorProviders.Providers.GetInterceptors())
					interceptor.Render(wrappedEvent, element);

				element.Save(writer);
			}

			return builder.ToString();
		}

		#endregion
	}
}
