using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clutch.Diagnostics
{
	public interface IAggregator
	{
		void AddValue(long value);

		long Count { get; }
		long Total { get; }

		double Result { get; }
	}
}
