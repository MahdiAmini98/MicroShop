using Common.EventBus.Configurations;
using Common.EventBus.Interfaces;
using Common.EventBus.Messages;
using Common.EventBus.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Common.EventBus.Extensions
{
    public static class RabbitMQServiceExtensions
    {
        public static IServiceCollection AddRabbitMQService(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var rabbitMqSection = configuration.GetSection("RabbitMqConfiguration");
            if (!rabbitMqSection.Exists())
            {
                throw new InvalidOperationException(
                    "RabbitMQ configuration section 'RabbitMq' not found in appsettings.json");
            }
          
            var rabbitMqConfig = new RabbitMqConfiguration
            {
                Hostname = rabbitMqSection["Hostname"] ?? "localhost",
                UserName = rabbitMqSection["UserName"] ?? "guest",
                Password = rabbitMqSection["Password"] ?? "guest",
                Port = int.TryParse(rabbitMqSection["Port"], out var p) ? p : 5672,
                RetryCount = int.TryParse(rabbitMqSection["RetryCount"], out var r) ? r : 5,
                AutomaticRecoveryEnabled = bool.TryParse(rabbitMqSection["AutomaticRecoveryEnabled"], out var a) ? a : true,
                NetworkRecoveryIntervalSeconds = int.TryParse(rabbitMqSection["NetworkRecoveryIntervalSeconds"], out var n) ? n : 10,
                RequestedHeartbeatSeconds = int.TryParse(rabbitMqSection["RequestedHeartbeatSeconds"], out var h) ? h : 30
            };

            // ثبت تنظیمات
            services.AddSingleton(rabbitMqConfig);

            // ثبت MessageBus
            services.AddSingleton<IMessageBus, RabbitMQMessageBusService>();

            return services;
        }

        /// <summary>
        /// ثبت و راه‌اندازی Consumer برای یک صف مشخص
        /// </summary>
        /// <typeparam name="TMessage">نوع پیام</typeparam>
        /// <typeparam name="THandler">نوع Handler</typeparam>
        /// <param name="services">کالکشن سرویس‌ها</param>
        /// <param name="queueName">نام صف</param>
        /// <returns></returns>
        public static IServiceCollection AddRabbitMQConsumer<TMessage, THandler>(
            this IServiceCollection services,
            string queueName)
            where TMessage : BaseMessage
            where THandler : class, IMessageHandler<TMessage>
        {
            // ثبت Handler در DI
            services.AddScoped<IMessageHandler<TMessage>, THandler>();

            // ثبت Consumer به عنوان HostedService
            services.AddHostedService(sp =>
                new RabbitMQConsumerService<TMessage, THandler>(sp, queueName));

            return services;
        }
    }
}