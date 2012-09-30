using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Clutch.Diagnostics.Tests
{
	public class DiagnosticsContextTest
	{
		[Fact]
		public void Can_compute_correct_average()
		{
			var context = new DiagnosticsContext(() => new AverageAggregator());

			context.PushResult(1);
			context.PushResult(5);

			context.Enabled = false;
			context.PushResult(200);

			Assert.Equal(3, context[""]);
		}

		[Fact]
		public void Can_compute_correct_moving_average()
		{
			var context = new DiagnosticsContext(() => new MovingAverageAggregator(history: 2));

			context.PushResult(1);
			context.PushResult(5);

			Assert.Equal(3, context[""]);

			context.PushResult(5);

			Assert.Equal(5, context[""]);

			context.PushResult(1);

			Assert.Equal(3, context[""]);
		}
	}
}
