using System;
using System.Xml.Linq;

namespace Clutch.Diagnostics.Logging.NLog.Interceptors
{
	public sealed class RunIdInterceptor : IXmlLogEventInterceptor
	{
		private const string KEY_RUN_ID = "runId";

		private static readonly string RunId = Guid.NewGuid().ToString();

		private RunIdInterceptor()
		{ }

		#region ILogEventInterceptor

		public void Prepare(global::NLog.LogEventInfo logEvent)
		{
			logEvent.Properties[KEY_RUN_ID] = RunId;
		}

		public void Render(global::NLog.LogEventInfo logEvent, XElement Run)
		{
			Run.Add(new XAttribute("runId", logEvent.Properties[KEY_RUN_ID]));
		}

		#endregion

		#region Static members

		public static readonly RunIdInterceptor Instance = new RunIdInterceptor();

		#endregion
	}
}
