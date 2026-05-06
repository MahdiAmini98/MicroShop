using Common.EventBus.Interfaces;
using Common.EventBus.Messages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Common.EventBus.Services
{
    /// <summary>
    /// سرویس پس‌زمینه برای اجرای Consumer
    /// </summary>
    public class RabbitMQConsumerService<TMessage, THandler> : BackgroundService
        where TMessage : BaseMessage
        where THandler : class, IMessageHandler<TMessage>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly string _queueName;
        private readonly ILogger<RabbitMQConsumerService<TMessage, THandler>> _logger;

        public RabbitMQConsumerService(IServiceProvider serviceProvider, string queueName)
        {
            _serviceProvider = serviceProvider;
            _queueName = queueName;
            _logger = serviceProvider.GetRequiredService<ILogger<RabbitMQConsumerService<TMessage, THandler>>>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Starting consumer for queue: {QueueName}", _queueName);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var messageBus = _serviceProvider.GetRequiredService<IMessageBus>();

                    await messageBus.StartConsumerAsync<TMessage, THandler>(_queueName, stoppingToken);

                    // نگه داشتن سرویس تا زمان لغو
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        await Task.Delay(1000, stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in consumer loop, restarting in 5 seconds...");
                    await Task.Delay(5000, stoppingToken);
                }
            }
        }
    }
}