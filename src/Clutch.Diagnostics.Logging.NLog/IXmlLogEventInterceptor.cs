using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Clutch.Diagnostics.Logging.NLog
{
	public interface IXmlLogEventInterceptor
	{
		void Prepare(global::NLog.LogEventInfo logEvent);
		void Render(global::NLog.LogEventInfo logEvent, XElement message);
	}
}
