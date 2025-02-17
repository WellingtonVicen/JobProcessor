namespace JobProcessor.Application.Ports
{
    public interface IMessageQueue
    {
        Task PublishAsync(string message, CancellationToken cancellationToken);
        Task ConsumeAsync(CancellationToken cancellationToken);
    }
}
