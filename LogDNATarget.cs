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
        private IApiClient apiClient;
        private ConfigurationManager config;

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
            config = new ConfigurationManager(Key);

            if (!string.IsNullOrEmpty(HostName))
            {
                config.HostName = HostName;
            }

            if (Tags.Any())
            {
                config.Tags = Tags;
            }

            apiClient = new HttpApiClient(config);
            apiClient.Connect();
            base.InitializeTarget();
        }

        protected override void CloseTarget()
        {
            apiClient.Disconnect();
            base.CloseTarget();
        }

        protected override void Write(LogEventInfo logEvent)
        {
            var message = $"{logEvent.TimeStamp:yyyy-MM-dd HH:mm:ss} {GetLevel(logEvent.Level)} {logEvent.Message}";
            apiClient.AddLine(new LogLine(ApplicationName, message, logEvent.TimeStamp));
        }

        private static string GetLevel(LogLevel level)
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
