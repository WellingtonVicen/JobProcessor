namespace JobProcessor.Application.Ports
{
    public interface IMessageQueueListener
    {
        Task HandleMessageAsync(string message, CancellationToken cancellationToken);
    }
}
