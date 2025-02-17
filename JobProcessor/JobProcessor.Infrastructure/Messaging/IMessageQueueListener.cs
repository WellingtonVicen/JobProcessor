namespace JobProcessor.Infrastructure.Messaging
{
    public interface IMessageQueueListener
    {
        Task ConsumeAsync(CancellationToken cancellationToken);
    }
}
