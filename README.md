# LogDNA.NLog

A LogDNA target for [NLog](http://nlog-project.org/).

## Getting Started

The easiest way to get started is to install the Nuget package:

```
Install-Package EATools.LogDNA.NLog
```

Then you will need to configure NLog, either via a config file or via code, specifying as a minimum your LogDNA key:
```xml
<nlog>
  <extensions>
    <add assembly="EATools.LogDNA.NLog"/>
  </extensions>
  <targets>
    <target name="logDna" type="LogDNATarget" Key="your-logdna-key"/>
  </targets>
  <rules>
    <logger name="*" minlevel="Debug" writeTo="logDna" />
  </rules>
</nlog>
```

```c#
var logDna = new LogDNATarget
{
    Key = "your-logdna-key"
};

var loggingConfig = new LoggingConfiguration();
loggingConfig.AddTarget("logDna", logDna);

var logDnaRule = new LoggingRule("*", LogLevel.Debug, logDna);
loggingConfig.LoggingRules.Add(logDnaRule);
```

## Optional configuration
The LogDNATarget object can also accept the following properties:

* ApplicationName; and
* HostName.

These can be specified within the ```<target>``` element in the config file or directly on the LogDNATarget object via code.

### ApplicationName
When specified, the App Name in LogDNA will be a combination of the ```ApplicationName``` and the Logger's name. For example, an ```ApplicationName``` of "MyApp" will result in the following App Name when called from a Logger with a name of "MyClass":

```
MyApp: MyClass
```

If the ```ApplicationName``` value isn't specified, only the Logger name will be used:

```
MyClass
```

### HostName

When a value is specified, the value will be sent to LogDNA as the host name. If not specified, the value of ```Environment.MachineName``` will be used instead.
