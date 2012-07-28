using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Clutch.Diagnostics.Logging
{
	public interface ILogEventInterceptor
	{
		void Prepare(ILogEvent logEvent);
		void Render(ILogEvent logEvent, XElement message);
	}
}
