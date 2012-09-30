using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clutch.Diagnostics
{
	public class AverageAggregator : AggregatorBase
	{
		public AverageAggregator()
		{ }

		public override double Result
		{
			get
			{
				if (Count <= 0)
					return 0;

				return Total / Count;
			}
		}
	}
}
