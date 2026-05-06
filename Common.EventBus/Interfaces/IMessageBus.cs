using Common.EventBus.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.EventBus.Interfaces
{
    public interface IMessageBus
    {
        /// <summary>
        /// ارسال پیام به یک صف مشخص
        /// </summary>
        /// <param name="message">پیام (از نوع BaseMessage)</param>
        /// <param name="queueName">نام صف مقصد</param>
        Task PublishAsync<TMessage>(TMessage message, string queueName) where TMessage : BaseMessage;

        /// <summary>
        /// اشتراک در صف و دریافت پیام (Consumer)
        /// </summary>
        Task SubscribeAsync<TMessage, THandler>(string queueName, CancellationToken cancellationToken = default)
            where TMessage : BaseMessage
            where THandler : IMessageHandler<TMessage>;

        /// <summary>
        /// شروع Consumer به عنوان سرویس پس‌زمینه
        /// </summary>
        Task StartConsumerAsync<TMessage, THandler>(string queueName, CancellationToken cancellationToken = default)
            where TMessage : BaseMessage
            where THandler : IMessageHandler<TMessage>;
    }
}
