using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clutch.Diagnostics
{
	public class MovingAverageAggregator : AverageAggregator
	{
		public MovingAverageAggregator(int history = 30)
		{
			if (history <= 0)
				throw new ArgumentOutOfRangeException("history");

			values = new long[history];
			position = 0;
		}

		private long[] values;
		private int position;

		public long this[int index]
		{
			get { return values[index]; }
		}

		public override void AddValue(long value)
		{
			base.AddValue(value);

			Total -= values[position];
			Count = Math.Min(Count, values.Length);

			values[position] = value;
			position = (position + 1) % values.Length;
		}
	}
}
