using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace Clutch.Diagnostics.Logging.NLog
{
	public class LogEventWrapper : ILogEvent
	{
		public LogEventWrapper(LogEventInfo logEvent)
		{
			this.logEvent = logEvent;
		}

		private LogEventInfo logEvent;

		public bool IsSet(string key)
		{
			return logEvent.Properties.ContainsKey(key);
		}

		public void Set(string key, object value)
		{
			logEvent.Properties[key] = value;
		}

		public object Get(string key)
		{
			return logEvent.Properties[key];
		}

		public bool TryGet(string key, out object value)
		{
			return logEvent.Properties.TryGetValue(key, out value);
		}

		public object TryGet(string key, object @default)
		{
			object value;
			if (TryGet(key, out value))
				return value;
			return @default;
		}
	}
}
