using System.Threading;
using System.Xml.Linq;

namespace Clutch.Diagnostics.Logging.NLog.Interceptors
{
	public sealed class IdentityInterceptor : IXmlLogEventInterceptor
	{
		private const string KEY_IDENTITY_TYPE = "identity.type";
		private const string KEY_IDENTITY_AUTHENTICATED = "identity.authenticated";
		private const string KEY_IDENTITY_NAME = "identity.name";

		private IdentityInterceptor()
		{ }

		#region ILogEventInterceptor

		public void Prepare(global::NLog.LogEventInfo logEvent)
		{
			logEvent.Properties[KEY_IDENTITY_NAME] = Thread.CurrentPrincipal.Identity.Name;
			logEvent.Properties[KEY_IDENTITY_AUTHENTICATED] = Thread.CurrentPrincipal.Identity.IsAuthenticated;
			logEvent.Properties[KEY_IDENTITY_TYPE] = Thread.CurrentPrincipal.Identity.AuthenticationType;
		}

		public void Render(global::NLog.LogEventInfo logEvent, XElement message)
		{
			message.Add(new XElement("identity",
				new XAttribute("name", logEvent.Properties[KEY_IDENTITY_NAME]),
				new XAttribute("authenticated", logEvent.Properties[KEY_IDENTITY_AUTHENTICATED]),
				new XAttribute("type", logEvent.Properties[KEY_IDENTITY_TYPE])
			));
		}

		#endregion

		#region Static members

		public static readonly IdentityInterceptor Instance = new IdentityInterceptor();

		#endregion
	}
}
