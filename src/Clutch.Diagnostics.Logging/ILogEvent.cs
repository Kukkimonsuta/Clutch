using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clutch.Diagnostics.Logging
{
	public interface ILogEvent
	{
		bool IsSet(string key);
		void Set(string key, object value);
		object Get(string key);
		bool TryGet(string key, out object value);
		object TryGet(string key, object @default);
	}
}
