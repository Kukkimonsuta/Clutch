using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clutch.Runtime
{
	public abstract class BootstrapSubscriber
	{
		public virtual void Startup() { }
		public virtual void OpenScope() { }
		public virtual void CloseScope() { }
		public virtual void Shutdown() { }
	}
}
