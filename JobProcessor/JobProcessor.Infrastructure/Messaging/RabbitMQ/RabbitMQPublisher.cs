using RabbitMQ.Client;
using System.Text;
namespace JobProcessor.Infrastructure.Messaging.RabbitMQ
{
    public class RabbitMQPublisher : IMessageQueuePublisher
    {
        private readonly IConnection _connection;
        private readonly string _queueName;

        public RabbitMQPublisher(IConnection connection, string queueName)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _queueName = queueName ?? throw new ArgumentNullException(nameof(queueName));
        }

        public Task PublishAsync(string message, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException("Message cannot be null or empty.", nameof(message));
            }

            // Verifica se o cancelamento foi solicitado antes de processar
            cancellationToken.ThrowIfCancellationRequested();

            using var channel = _connection.CreateModel();

            // Declaração da fila (caso ainda não tenha sido criada)
            channel.QueueDeclare(queue: _queueName,
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var body = Encoding.UTF8.GetBytes(message);

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true; // Garante que a mensagem seja persistente

            // Publica a mensagem na fila
            channel.BasicPublish(exchange: "",
                                 routingKey: _queueName,
                                 basicProperties: properties,
                                 body: body);

            // Log da mensagem enviada
            Console.WriteLine($"[x] Sent {message}");

            return Task.CompletedTask;
        }
    }
}