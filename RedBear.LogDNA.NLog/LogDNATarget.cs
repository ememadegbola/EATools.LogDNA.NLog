using NLog;
using NLog.Config;
using NLog.Targets;

namespace RedBear.LogDNA.NLog
{
    /// <summary>
    /// NLog target for LogDNA.
    /// </summary>
    /// <seealso cref="NLog.Targets.Target" />
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
        [RequiredParameter]
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

        protected override void InitializeTarget()
        {
            var config = new Config(ApplicationName, Key);

            if (!string.IsNullOrEmpty(HostName))
            {
                config.HostName = HostName;
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
            var message = $"{logEvent.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss")} [{logEvent.Level.ToString().ToUpper()}] {logEvent.Message}";
            ApiClient.AddLine(new LogLine(logEvent.LoggerName, message, logEvent.TimeStamp));
        }
    }
}
