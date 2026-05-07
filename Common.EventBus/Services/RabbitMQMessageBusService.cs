using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Common.EventBus.Configurations;
using Common.EventBus.Interfaces;
using Common.EventBus.Messages;
using System.Text;
using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;

using RabbitMQ.Client.Events;
namespace Common.EventBus.Services
{
    /// <summary>
    /// پیاده‌سازی کامل RabbitMQ شامل Producer و Consumer
    /// </summary>
    public class RabbitMQMessageBusService : IMessageBus, IDisposable
    {
        private readonly RabbitMqConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<RabbitMQMessageBusService> _logger;
        private readonly ConcurrentDictionary<string, Task> _consumerTasks;

        private IConnection _connection;
        private IChannel _channel;
        private bool _disposed;

        public RabbitMQMessageBusService(
            RabbitMqConfiguration configuration,
            IServiceProvider serviceProvider,
            ILogger<RabbitMQMessageBusService> logger)
        {
            _configuration = configuration;
            _serviceProvider = serviceProvider;
            _logger = logger;
            _consumerTasks = new ConcurrentDictionary<string, Task>();

            InitializeConnection().GetAwaiter().GetResult();
        }

        #region Connection Management
        private async Task InitializeConnection()
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = _configuration.Hostname,
                    UserName = _configuration.UserName,
                    Password = _configuration.Password,
                    Port = _configuration.Port,
                    AutomaticRecoveryEnabled = _configuration.AutomaticRecoveryEnabled,
                    NetworkRecoveryInterval = TimeSpan.FromSeconds(_configuration.NetworkRecoveryIntervalSeconds),
                    RequestedHeartbeat = TimeSpan.FromSeconds(_configuration.RequestedHeartbeatSeconds),
                    ConsumerDispatchConcurrency = 1 // تعداد پیام‌هایی که همزمان پردازش می‌شوند
                };

                _connection = await factory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync();

                _logger.LogInformation("RabbitMQ connection established to {Hostname}:{Port}",
                    _configuration.Hostname, _configuration.Port);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to establish RabbitMQ connection");
                throw;
            }
        }

        private async Task EnsureConnectionAsync()
        {
            if (_connection == null || !_connection.IsOpen || _channel == null || !_channel.IsOpen)
            {
                await InitializeConnection();
            }
        }

        #endregion

        #region Direct Producer And Consumer

        #region Direct Producer

        public virtual async Task PublishAsync<TMessage>(TMessage message, string queueName) where TMessage : BaseMessage
        {
            await EnsureConnectionAsync();

            try
            {
                message.MessageType = typeof(TMessage).Name;

                var json = JsonConvert.SerializeObject(message);
                var body = Encoding.UTF8.GetBytes(json);

                var properties = new BasicProperties
                {
                    Persistent = true, // پیام روی دیسک ذخیره شود
                    DeliveryMode = DeliveryModes.Persistent,
                    ContentType = "application/json", // نوع داده پیام (JSON)
                    MessageId = message.MessageId.ToString(), // شناسه یکتا برای رهگیری
                    Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds()) // زمان ارسال برای دیباگ
                };

                _logger.LogDebug("Publishing message {MessageType} with Id {MessageId} to queue {QueueName}",
                    message.MessageType, message.MessageId, queueName);

                await _channel.BasicPublishAsync(
                    exchange: "",
                    routingKey: queueName,
                    mandatory: true,
                    basicProperties: properties,
                    body: body);

                _logger.LogInformation("Message {MessageId} published successfully", message.MessageId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish message {MessageType}", typeof(TMessage).Name);
                throw;
            }
        }

        #endregion

        #region Direct Consumer

        /// <summary>
        /// اشتراک در یک صف و پردازش پیام‌ها با Handler مشخص
        /// </summary>
        /// <typeparam name="TMessage">نوع پیام</typeparam>
        /// <typeparam name="THandler">نوع Handler (پیاده‌کننده IMessageHandler)</typeparam>
        /// <param name="queueName">نام صف</param>
        /// <param name="cancellationToken">توکن لغو</param>
        public virtual async Task SubscribeAsync<TMessage, THandler>(string queueName, CancellationToken cancellationToken = default)
            where TMessage : BaseMessage
            where THandler : IMessageHandler<TMessage>
        {
            await EnsureConnectionAsync();

            // ایجاد صف اگر وجود نداشته باشد
            await _channel.QueueDeclareAsync(
                queue: queueName, // نام صف (مثال: "basket-checkout-queue")
                durable: true, // آیا پیام‌ها روی دیسک ذخیره شوند؟
                exclusive: false, // آیا فقط توسط این اتصال استفاده شود؟
                autoDelete: false, // آیا با خالی شدن خودکار حذف شود؟
                arguments: null);

            // ایجاد مصرف‌کننده => این یک مصرف‌کننده ناهمزمان است که به محض رسیدن پیام، رویداد  
            //را فعال می‌کند  Received 
            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (sender, args) =>
            {
                await ProcessMessage<TMessage, THandler>(args, cancellationToken);
            };

            await _channel.BasicConsumeAsync(
                queue: queueName,
                autoAck: false,
                consumer: consumer);

            _logger.LogInformation("Subscribed to queue '{QueueName}' with handler {HandlerName}",
                queueName, typeof(THandler).Name);
        }

        /// <summary>
        /// پردازش یک پیام دریافتی
        /// </summary>
        public virtual async Task ProcessMessage<TMessage, THandler>(BasicDeliverEventArgs args, CancellationToken cancellationToken)
            where TMessage : BaseMessage
            where THandler : IMessageHandler<TMessage>
        {
            var messageId = args.BasicProperties?.MessageId ?? "unknown";

            try
            {
                var body = args.Body.ToArray();
                var messageJson = Encoding.UTF8.GetString(body);

                _logger.LogDebug("Received message {MessageId}, content: {Content}", messageId, messageJson);

                var message = JsonConvert.DeserializeObject<TMessage>(messageJson);

                using var scope = _serviceProvider.CreateScope();

                var handler = scope.ServiceProvider.GetRequiredService<IMessageHandler<TMessage>>();

                if (handler == null)
                {
                    _logger.LogCritical("Handler {HandlerType} not registered! Cannot process message.", typeof(THandler).Name);
                    await _channel.BasicNackAsync(args.DeliveryTag, false, false);
                    return;
                }

                await handler.HandleAsync(message, cancellationToken);

                // تأیید موفقیت آمیز بودن پردازش
                await _channel.BasicAckAsync(args.DeliveryTag, false);

                _logger.LogInformation("Message {MessageId} processed successfully", messageId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message {MessageId}", messageId);

                // رد کردن پیام (بدون requeue) برای جلوگیری از حلقه بی‌نهایت
                await _channel.BasicNackAsync(args.DeliveryTag, false, false);
            }
        }

        /// <summary>
        /// شروع یک Consumer به عنوان BackgroundService
        /// </summary>
        public Task StartConsumerAsync<TMessage, THandler>(string queueName, CancellationToken cancellationToken = default)
            where TMessage : BaseMessage
            where THandler : IMessageHandler<TMessage>
        {
            var consumerKey = $"{queueName}_{typeof(THandler).Name}";

            if (_consumerTasks.ContainsKey(consumerKey))
            {
                _logger.LogWarning("Consumer for queue '{QueueName}' is already running", queueName);
                return Task.CompletedTask;
            }

            var consumerTask = Task.Run(async () =>
            {
                await SubscribeAsync<TMessage, THandler>(queueName, cancellationToken);

                // نگه داشتن consumer تا زمان لغو
                while (!cancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(1000, cancellationToken);
                }
            }, cancellationToken);

            _consumerTasks.TryAdd(consumerKey, consumerTask);

            _logger.LogInformation("Started consumer for queue '{QueueName}'", queueName);

            return Task.CompletedTask;
        }

        #endregion

        #endregion

        #region Fanout Producer And Consumer

        #region Fanout Producer

        /// <summary>
        /// ارسال پیام Fanout (به همه مشترکین)
        /// </summary>
        public virtual async Task PublishFanoutAsync<TMessage>(TMessage message, string exchangeName) where TMessage : BaseMessage
        {
            await EnsureConnectionAsync();

            try
            {
                message.MessageType = typeof(TMessage).Name;

                // اعلان Exchange از نوع Fanout (اگر وجود نداشته باشد)
                await _channel.ExchangeDeclareAsync(
                    exchange: exchangeName,
                    type: ExchangeType.Fanout,
                    durable: true,
                    autoDelete: false);

                var json = JsonConvert.SerializeObject(message);
                var body = Encoding.UTF8.GetBytes(json);

                var properties = new BasicProperties
                {
                    Persistent = true,
                    DeliveryMode = DeliveryModes.Persistent,
                    ContentType = "application/json",
                    MessageId = message.MessageId.ToString(),
                    Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds())
                };

                _logger.LogDebug("Publishing fanout message {MessageType} to exchange {ExchangeName}",
                    message.MessageType, exchangeName);

                await _channel.BasicPublishAsync(
                    exchange: exchangeName,
                    routingKey: "",          // در Fanout، routingKey نادیده گرفته می‌شود
                    mandatory: false,
                    basicProperties: properties,
                    body: body);

                _logger.LogInformation("Fanout message {MessageId} published to exchange {ExchangeName}",
                    message.MessageId, exchangeName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish fanout message {MessageType}", typeof(TMessage).Name);
                throw;
            }
        }

        #endregion

        #region Fanout Consumer
        /// <summary>
        /// اشتراک در Exchange از نوع Fanout (هر سرویس یک صف اختصاصی و خودکار می‌سازد)
        /// </summary>
        /// 

        public virtual async Task SubscribeFanoutAsync<TMessage, THandler>(string exchangeName, string? queueName = null, CancellationToken cancellationToken = default)
            where TMessage : BaseMessage
            where THandler : IMessageHandler<TMessage>
        {
            await EnsureConnectionAsync();

            // 1. اعلان Exchange از نوع Fanout
            await _channel.ExchangeDeclareAsync(
                exchange: exchangeName,
                type: ExchangeType.Fanout,
                durable: true,
                autoDelete: false);

            // 2. تصمیم‌گیری درباره صف
            string actualQueueName;
            if (string.IsNullOrWhiteSpace(queueName))
            {
                // حالت تصادفی (Anonymous) - مناسب کش محلی
                var declareResult = await _channel.QueueDeclareAsync(
                    queue: "", // نام خالی → RabbitMQ یک نام تصادفی می‌دهد
                    durable: false,
                    exclusive: true,
                    autoDelete: true,
                    arguments: null);
                actualQueueName = declareResult.QueueName;
                _logger.LogInformation("Created temporary queue '{QueueName}' for fanout (cache scenario)", actualQueueName);
            }
            else
            {
                // حالت Named Queue - مناسب دیتابیس مشترک (Compelling Consumers)
                await _channel.QueueDeclareAsync(
                    queue: queueName,
                    durable: true,      
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);
                actualQueueName = queueName;
                _logger.LogInformation("Using named queue '{QueueName}' for fanout (database scenario)", actualQueueName);
            }

            _logger.LogInformation("Created temporary queue '{QueueName}' for fanout subscription to exchange '{ExchangeName}'",
                queueName, exchangeName);


            // 3. Bind کردن صف به Exchange
            await _channel.QueueBindAsync(actualQueueName, exchangeName, routingKey: "");

            // 4. شروع مصرف (Consumer)
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (sender, args) =>
            {
                await ProcessFanoutMessage<TMessage, THandler>(args, cancellationToken);
            };

            await _channel.BasicConsumeAsync(queue: actualQueueName, autoAck: false, consumer: consumer);
        }

        /// <summary>
        /// پردازش پیام دریافتی از Fanout Exchange
        /// </summary>
        private async Task ProcessFanoutMessage<TMessage, THandler>(BasicDeliverEventArgs args, CancellationToken cancellationToken)
            where TMessage : BaseMessage
            where THandler : IMessageHandler<TMessage>
        {
            var messageId = args.BasicProperties?.MessageId ?? "unknown";

            try
            {
                var body = args.Body.ToArray();
                var messageJson = Encoding.UTF8.GetString(body);
                var message = JsonConvert.DeserializeObject<TMessage>(messageJson);

                if (message == null)
                {
                    _logger.LogWarning("Failed to deserialize fanout message {MessageId}", messageId);
                    await _channel.BasicNackAsync(args.DeliveryTag, false, false);
                    return;
                }

                using var scope = _serviceProvider.CreateScope();
                var handler = scope.ServiceProvider.GetRequiredService<IMessageHandler<TMessage>>();
                await handler.HandleAsync(message, cancellationToken);

                await _channel.BasicAckAsync(args.DeliveryTag, false);
                _logger.LogInformation("Fanout message {MessageId} processed successfully", messageId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing fanout message {MessageId}", messageId);
                await _channel.BasicNackAsync(args.DeliveryTag, false, false);
            }
        }

        #endregion

        #endregion


        #region Dispose
        public void Dispose()
        {
            if (!_disposed)
            {
                _channel?.CloseAsync().GetAwaiter().GetResult();
                _channel?.Dispose();
                _connection?.CloseAsync().GetAwaiter().GetResult();
                _connection?.Dispose();
                _disposed = true;

                _logger.LogInformation("RabbitMQ resources disposed");
            }
        }
        #endregion
    }
}
