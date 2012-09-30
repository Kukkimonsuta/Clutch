using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Clutch.Diagnostics
{
	public class DiagnosticsFragment : IDisposable
	{
		internal DiagnosticsFragment(DiagnosticsContext context, string category)
		{
			if (context == null)
				throw new ArgumentNullException("context");
			if (category == null)
				throw new ArgumentNullException("category");

			this.context = context;
			this.category = category;

			stopwatch = Stopwatch.StartNew();
		}

		private DiagnosticsContext context;
		private string category;
		private Stopwatch stopwatch;

		public void Dispose()
		{
			if (!stopwatch.IsRunning)
				throw new InvalidOperationException("Cannot dispose disposed diagnostics fragment");

			stopwatch.Stop();

			context.PushResult(stopwatch.ElapsedMilliseconds, category: category);
		}
	}
}
