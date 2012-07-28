using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using System.IO;

namespace Clutch.Diagnostics.Logging.Nlog.Tests
{
	public class XmlEventRendererTests
	{
		[Fact]
		public void Can_write_to_log_using_default_methods()
		{
			if (File.Exists("log.txt"))
				File.Delete("log.txt");

			var logger = global::NLog.LogManager.GetCurrentClassLogger();

			logger.Info("Hello!");
			logger.InfoException("Hello!", new Exception("Test exception"));

			var test = File.ReadAllText("log.txt");

			Assert.True(File.Exists("log.txt"));
			Assert.True(test.Length > 0);
		}
	}
}
