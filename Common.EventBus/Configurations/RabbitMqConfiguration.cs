namespace Common.EventBus.Configurations
{
    public class RabbitMqConfiguration
    {
        public string Hostname { get; set; } = "localhost";
        public string UserName { get; set; } = "guest";
        public string Password { get; set; } = "guest";
        public int Port { get; set; } = 5672;
        public int RetryCount { get; set; } = 5;
        public bool AutomaticRecoveryEnabled { get; set; } = true; 
        public int NetworkRecoveryIntervalSeconds { get; set; } = 10;
        public int RequestedHeartbeatSeconds { get; set; } = 30;
    }
}
