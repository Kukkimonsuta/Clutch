# Clutch

## Clutch.Diagnostics.EntityFramework

Provides api for tracing EntityFramework sql commands.

Example listener:

```c#
public class NLogDbTracingListener : IDbTracingListener
{
  private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

  void IDbTracingListener.CommandExecuted(DbConnection connection, DbCommand command, object result, TimeSpan duration)
  { }
  
  void IDbTracingListener.CommandExecuting(DbConnection connection, DbCommand command)
  { }
  
  void IDbTracingListener.CommandFailed(DbConnection connection, DbCommand command, Exception exception, TimeSpan duration)
  { }
  
  void IDbTracingListener.CommandFinished(DbConnection connection, DbCommand command, object result, TimeSpan duration)
  {
    logger.Trace("-- time: {0}{1}{2}", duration, Environment.NewLine, command.ToTraceString());
  }
}
```

Example configuration for web project:

```c#
protected void Application_Start()
{
    DbTracing.Initialize();
    DbTracing.AddListener(new NLogDbTracingListener());
}
```

## Clutch.Diagnostics.Logging.NLog

Extends log messages of NLog with Clutch.Diagnostics.Logging and writes to log file using xml.

Example NLog configuration:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd"
      xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
      autoReload="true">

  <extensions>
    <add assembly="Clutch.Diagnostics.Logging.NLog" />
  </extensions>

  <targets>
    <target xsi:type="File" name="file" encoding="UTF-8" fileName="${basedir}/App_Data/logs/log.txt" archiveNumbering="Rolling"
            maxArchiveFiles="28" archiveEvery="Day" archiveFileName="${basedir}/App_Data/logs/log_{##}.txt"
            createDirs="true" autoFlush="true">
      <layout xsi:type="ClutchXml" />
    </target>

    <target name="debugger" xsi:type="Debugger"
            layout="${longdate}|${level:uppercase=true}|${logger}|${message}|${exception:format=tostring}" />
  </targets>

  <rules>
    <logger name="*" minlevel="Debug" writeTo="file" />
    <logger name="*" minlevel="Trace" writeTo="debugger" />
  </rules>
</nlog>
```