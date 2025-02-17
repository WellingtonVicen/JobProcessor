using JobProcessor.Application.Ports;

namespace JobProcessor.Infrastructure.Messaging.RabbitMQ
{
    public class RabbitMQService : IMessageQueue
    {
        private readonly IMessageQueueListener _messageQueueListener;
        private readonly IMessageQueuePublisher _messageQueuePublisher;

        public RabbitMQService(IMessageQueueListener messageQueueListener, IMessageQueuePublisher messageQueuePublisher)
        {
            _messageQueueListener = messageQueueListener;
            _messageQueuePublisher = messageQueuePublisher;
        }

        public async Task PublishAsync(string message, CancellationToken cancellationToken)
        {
            await _messageQueuePublisher.PublishAsync(message, cancellationToken);
        }

        public async Task ConsumeAsync(CancellationToken cancellationToken)
        {
            await _messageQueueListener.ConsumeAsync(cancellationToken);
        }
    }
}
