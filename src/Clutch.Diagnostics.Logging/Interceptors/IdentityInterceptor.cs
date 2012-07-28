using System.Threading;
using System.Xml.Linq;

namespace Clutch.Diagnostics.Logging.Interceptors
{
	public sealed class IdentityInterceptor : ILogEventInterceptor
	{
		private const string KEY_IDENTITY_TYPE = "identity.type";
		private const string KEY_IDENTITY_AUTHENTICATED = "identity.authenticated";
		private const string KEY_IDENTITY_NAME = "identity.name";

		private IdentityInterceptor()
		{ }

		#region ILogEventInterceptor

		public void Prepare(ILogEvent logEvent)
		{
			var identity = Thread.CurrentPrincipal.Identity;

			logEvent.Set(KEY_IDENTITY_NAME, identity.Name);
			logEvent.Set(KEY_IDENTITY_AUTHENTICATED, identity.IsAuthenticated);
			logEvent.Set(KEY_IDENTITY_TYPE, identity.AuthenticationType);
		}

		public void Render(ILogEvent logEvent, XElement message)
		{
			message.Add(new XElement("identity",
				new XAttribute("name", logEvent.TryGet(KEY_IDENTITY_NAME, null)),
				new XAttribute("authenticated", logEvent.TryGet(KEY_IDENTITY_AUTHENTICATED, null)),
				new XAttribute("type", logEvent.TryGet(KEY_IDENTITY_TYPE, null))
			));
		}

		#endregion

		#region Static members

		public static readonly IdentityInterceptor Instance = new IdentityInterceptor();

		#endregion
	}
}
