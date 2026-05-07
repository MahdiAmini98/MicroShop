using Common.EventBus.Interfaces;
using Common.EventBus.Messages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Common.EventBus.Services
{
    public class RabbitMQFanoutConsumerService<TMessage, THandler> : BackgroundService
      where TMessage : BaseMessage
      where THandler : class, IMessageHandler<TMessage>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly string _exchangeName;
        private readonly string? _queueName;       // جدید: نام صف (اختیاری)
        private readonly ILogger _logger;

        // ✅ سازنده برای سناریوی دیتابیس (Named Queue)
        public RabbitMQFanoutConsumerService(
            IServiceProvider serviceProvider,
            string exchangeName,
            string queueName)
           
        {
            _serviceProvider = serviceProvider;
            _exchangeName = exchangeName;
            _queueName = queueName;
            _logger = serviceProvider.GetRequiredService<ILogger<RabbitMQFanoutConsumerService<TMessage, THandler>>>();
        }

        // ✅ سازنده برای سناریوی کش (Anonymous Queue)
        public RabbitMQFanoutConsumerService(
            IServiceProvider serviceProvider,
            string exchangeName)
            : this(serviceProvider, exchangeName, null)
        {
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var messageBus = _serviceProvider.GetRequiredService<IMessageBus>();

                    // ✅ فراخوانی متد با پارامترهای اختیاری
                    await messageBus.SubscribeFanoutAsync<TMessage, THandler>(
                        exchangeName: _exchangeName,
                        queueName: _queueName,
                        cancellationToken: stoppingToken);

                    // نگه داشتن سرویس تا زمان لغو
                    await Task.Delay(Timeout.Infinite, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Fanout consumer error for exchange '{ExchangeName}', restarting in 5 seconds...", _exchangeName);
                    await Task.Delay(5000, stoppingToken);
                }
            }
        }
    }
}
