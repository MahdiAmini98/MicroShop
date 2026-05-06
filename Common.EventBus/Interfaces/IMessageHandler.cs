using Common.EventBus.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.EventBus.Interfaces
{
    /// <summary>
    /// پردازشگر پیام‌های دریافتی از RabbitMQ
    /// </summary>
    /// <typeparam name="TMessage">نوع پیام (از BaseMessage)</typeparam>
    public interface IMessageHandler<TMessage> where TMessage : BaseMessage
    {
        /// <summary>
        /// پردازش پیام دریافتی
        /// </summary>
        /// <param name="message">پیام دریافتی</param>
        /// <param name="cancellationToken">توکن لغو عملیات</param>
        Task HandleAsync(TMessage message, CancellationToken cancellationToken = default);
    }
}
