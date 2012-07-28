using System;
using System.Xml.Linq;

namespace Clutch.Diagnostics.Logging.Interceptors
{
	public sealed class MessageIdInterceptor : ILogEventInterceptor
	{
		private const string KEY_MESSAGE_ID = "id";

		private MessageIdInterceptor()
		{ }

		#region ILogEventInterceptor

		public void Prepare(ILogEvent logEvent)
		{
			logEvent.Set(KEY_MESSAGE_ID, Guid.NewGuid());
		}

		public void Render(ILogEvent logEvent, XElement message)
		{
			message.Add(new XAttribute("id", logEvent.TryGet(KEY_MESSAGE_ID, "")));
		}

		#endregion

		#region Static members

		public static readonly MessageIdInterceptor Instance = new MessageIdInterceptor();

		#endregion
	}
}
