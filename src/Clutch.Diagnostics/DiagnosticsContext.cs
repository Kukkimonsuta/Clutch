using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Clutch.Diagnostics
{
	public class DiagnosticsContext
	{
		public DiagnosticsContext(Func<IAggregator> aggregatorFactory)
		{
			this.aggregatorFactory = aggregatorFactory;
			this.sync = new object();

			results = new Dictionary<string, IAggregator>();
			enabled = true;
		}

		private Func<IAggregator> aggregatorFactory;
		private object sync;
		private volatile bool enabled;
		private Dictionary<string, IAggregator> results;

		public bool Enabled
		{
			get { return enabled; }
			set { enabled = value; }
		}

		public double this[string category]
		{
			get
			{
				lock (sync)
				{
					IAggregator aggregator;
					if (!results.TryGetValue(category, out aggregator))
						return 0;

					return aggregator.Result;
				}
			}
		}

		public void PushResult(long value, string category = "")
		{
			if (category == null)
				throw new ArgumentNullException("category");

			if (!enabled)
				return;

			lock (sync)
			{
				IAggregator aggregator;
				if (!results.TryGetValue(category, out aggregator))
					aggregator = results[category] = aggregatorFactory();

				aggregator.AddValue(value);
			}
		}

		public DiagnosticsFragment Measure(string category = "")
		{
			if (category == null)
				throw new ArgumentNullException("category");

			if (!enabled)
				return null;

			return new DiagnosticsFragment(this, category);
		}

		public override string ToString()
		{
			var builder = new StringBuilder();

			foreach (var aggregator in results)
				builder.AppendFormat("{0}: {1}{2}", aggregator.Key, TimeSpan.FromMilliseconds(aggregator.Value.Result), Environment.NewLine);

			return builder.ToString();
		}
	}
}
