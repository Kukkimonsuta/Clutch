using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clutch.Diagnostics
{
	public abstract class AggregatorBase : IAggregator
	{
		public AggregatorBase()
		{ }

		public long Count { get; protected set; }
		public long Total { get; protected set; }
		public long Min { get; protected set; }
		public long Max { get; protected set; }

		public abstract double Result { get; }

		public virtual void AddValue(long value)
		{
			Count++;
			Total += value;

			Min = Math.Min(Min, value);
			Max = Math.Max(Max, value);
		}
	}
}
