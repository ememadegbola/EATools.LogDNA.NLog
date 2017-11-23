using NLog;
using NLog.Config;
using NLog.Targets;
using System.Collections.Generic;
using System.Linq;

namespace RedBear.LogDNA.NLog
{
    /// <summary>
    /// NLog target for LogDNA.
    /// </summary>
    /// <seealso cref="Target" />
    [Target("LogDNA")]
    // ReSharper disable once InconsistentNaming
    public class LogDNATarget : Target
    {
        /// <summary>
        /// Gets or sets the name of the application that's emitting the content that's sent to LogDNA.
        /// </summary>
        /// <value>
        /// The name of the application.
        /// </value>
        public string ApplicationName { get; set; }

        /// <summary>
        /// Gets or sets the LogDNA key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        [RequiredParameter]
        public string Key { get; set; }

        public string HostName { get; set; }

        public List<string> Tags { get; set; }

        protected override void InitializeTarget()
        {
            var config = new Config(ApplicationName, Key);

            if (!string.IsNullOrEmpty(HostName))
            {
                config.HostName = HostName;
            }

            if (Tags.Any())
            {
                config.Tags = Tags;
            }

            ApiClient.Connect(config).Wait();
            base.InitializeTarget();
        }

        protected override void CloseTarget()
        {
            ApiClient.Disconnect();
            base.CloseTarget();
        }

        protected override void Write(LogEventInfo logEvent)
        {
            var logName = !string.IsNullOrEmpty(ApiClient.Config.ApplicationName) ? $"{ApiClient.Config.ApplicationName}: {logEvent.LoggerName}" : logEvent.LoggerName;
            var message = $"{logEvent.TimeStamp:yyyy-MM-dd HH:mm:ss} {GetLevel(logEvent.Level)} {logEvent.Message}";
            ApiClient.AddLine(new LogLine(logName, message, logEvent.TimeStamp));
        }

        private string GetLevel(LogLevel level)
        {
            if (level == LogLevel.Debug)
                return "DEBUG";

            if (level == LogLevel.Trace)
                return "TRACE";

            if (level == LogLevel.Info)
                return "INFO";

            if (level == LogLevel.Warn)
                return "WARN";

            if (level == LogLevel.Error)
                return "ERROR";

            return "FATAL";
        }
    }
}
