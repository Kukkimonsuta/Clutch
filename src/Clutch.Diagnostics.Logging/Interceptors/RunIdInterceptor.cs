using System;
using System.Xml.Linq;

namespace Clutch.Diagnostics.Logging.Interceptors
{
	public sealed class RunIdInterceptor : ILogEventInterceptor
	{
		private const string KEY_RUN_ID = "runId";

		private static readonly string RunId = Guid.NewGuid().ToString();

		private RunIdInterceptor()
		{ }

		#region ILogEventInterceptor

		public void Prepare(ILogEvent logEvent)
		{
			logEvent.Set(KEY_RUN_ID, RunId);
		}

		public void Render(ILogEvent logEvent, XElement Run)
		{
			Run.Add(new XAttribute("runId", logEvent.TryGet(KEY_RUN_ID, "")));
		}

		#endregion

		#region Static members

		public static readonly RunIdInterceptor Instance = new RunIdInterceptor();

		#endregion
	}
}
