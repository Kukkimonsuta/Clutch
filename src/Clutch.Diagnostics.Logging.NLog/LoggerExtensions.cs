using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Clutch.Diagnostics.Logging.NLog
{
	public static class LoggerExtensions
	{
		private static void Log(Logger logger, LogLevel level, Exception exception, string extendedInfo, string format, params object[] args)
		{
			var eventInfo = exception == null ? new LogEventInfo(level, logger.Name, CultureInfo.InvariantCulture, format, args)
				: new LogEventInfo(level, logger.Name, CultureInfo.InvariantCulture, format, args, exception);

			if (extendedInfo != null)
				eventInfo.Properties["extendedInfo"] = extendedInfo;

			logger.Log(eventInfo);
		}

		public static void TraceExtended(this Logger logger, string message, string extendedInfo)
		{
			Log(logger, LogLevel.Trace, null, extendedInfo, message);
		}

		public static void DebugExtended(this Logger logger, string message, string extendedInfo)
		{
			Log(logger, LogLevel.Debug, null, extendedInfo, message);
		}

		public static void InfoExtended(this Logger logger, string message, string extendedInfo)
		{
			Log(logger, LogLevel.Info, null, extendedInfo, message);
		}

		public static void WarnExtended(this Logger logger, string message, string extendedInfo)
		{
			Log(logger, LogLevel.Warn, null, extendedInfo, message);
		}

		public static void ErrorExtended(this Logger logger, string message, string extendedInfo)
		{
			Log(logger, LogLevel.Error, null, extendedInfo, message);
		}

		public static void FatalExtended(this Logger logger, string message, string extendedInfo)
		{
			Log(logger, LogLevel.Fatal, null, extendedInfo, message);
		}

		public static void TraceExceptionExtended(this Logger logger, string message, Exception exception, string extendedInfo)
		{
			Log(logger, LogLevel.Trace, exception, extendedInfo, message);
		}

		public static void DebugExceptionExtended(this Logger logger, string message, Exception exception, string extendedInfo)
		{
			Log(logger, LogLevel.Debug, exception, extendedInfo, message);
		}

		public static void InfoExceptionExtended(this Logger logger, string message, Exception exception, string extendedInfo)
		{
			Log(logger, LogLevel.Info, exception, extendedInfo, message);
		}

		public static void WarnExceptionExtended(this Logger logger, string message, Exception exception, string extendedInfo)
		{
			Log(logger, LogLevel.Warn, exception, extendedInfo, message);
		}

		public static void ErrorExceptionExtended(this Logger logger, string message, Exception exception, string extendedInfo)
		{
			Log(logger, LogLevel.Error, exception, extendedInfo, message);
		}

		public static void FatalExceptionExtended(this Logger logger, string message, Exception exception, string extendedInfo)
		{
			Log(logger, LogLevel.Fatal, exception, extendedInfo, message);
		}
	}
}
