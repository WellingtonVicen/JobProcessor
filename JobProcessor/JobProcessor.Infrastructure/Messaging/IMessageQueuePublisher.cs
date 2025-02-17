namespace JobProcessor.Infrastructure.Messaging
{
    public interface IMessageQueuePublisher
    {
        Task PublishAsync(string message, CancellationToken cancellationToken);
    }
}
