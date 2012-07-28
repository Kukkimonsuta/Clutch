using System;
using System.Xml.Linq;

namespace Clutch.Diagnostics.Logging.NLog.Interceptors
{
	public sealed class MessageIdInterceptor : IXmlLogEventInterceptor
	{
		private const string KEY_MESSAGE_ID = "id";

		private MessageIdInterceptor()
		{ }

		#region ILogEventInterceptor

		public void Prepare(global::NLog.LogEventInfo logEvent)
		{
			logEvent.Properties[KEY_MESSAGE_ID] = Guid.NewGuid();
		}

		public void Render(global::NLog.LogEventInfo logEvent, XElement message)
		{
			message.Add(new XAttribute("id", logEvent.Properties[KEY_MESSAGE_ID]));
		}

		#endregion

		#region Static members

		public static readonly MessageIdInterceptor Instance = new MessageIdInterceptor();

		#endregion
	}
}
