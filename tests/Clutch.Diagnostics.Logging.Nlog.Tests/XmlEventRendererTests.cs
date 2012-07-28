using System;
using System.IO;
using Xunit;

namespace Clutch.Diagnostics.Logging.Nlog.Tests
{
	public class XmlEventRendererTests
	{
		private static readonly global::NLog.Logger logger = global::NLog.LogManager.GetCurrentClassLogger();

		[Fact]
		public void Can_write_to_log_using_default_methods()
		{
			if (File.Exists("log.txt"))
				File.Delete("log.txt");

			for (var i = 0; i < 10; i++)
				logger.Info("Hello {0}!", i);

			try
			{
				throw new Exception("Test exception");
			}
			catch (Exception ex)
			{
				logger.ErrorException("It failed :(", ex);
			}

			var test = File.ReadAllText("log.txt");

			Assert.True(File.Exists("log.txt"));
			Assert.True(test.Length > 0);
		}
	}
}
