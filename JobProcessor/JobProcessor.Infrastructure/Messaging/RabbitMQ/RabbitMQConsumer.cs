using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace JobProcessor.Infrastructure.Messaging.RabbitMQ
{
    public class RabbitMQConsumer : IMessageQueueListener
    {
        private readonly IConnection _connection;
        private readonly string _queueName;

        public RabbitMQConsumer(IConnection connection, string queueName)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _queueName = queueName ?? throw new ArgumentNullException(nameof(queueName));
        }

        public async Task ConsumeAsync(CancellationToken cancellationToken)
        {
            using var channel = _connection.CreateModel();

            // Declaração da fila (caso ainda não tenha sido criada)
            channel.QueueDeclare(queue: _queueName,
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                Console.WriteLine($"[x] Received {message}");

                // Simula o processamento assíncrono da mensagem
                await Task.Yield();
            };

            // Consumir mensagens da fila
            channel.BasicConsume(
                queue: _queueName,
                autoAck: true,
                consumer: consumer
            );

            // Aguarda até o cancelamento ser solicitado
            try
            {
                await Task.Delay(Timeout.Infinite, cancellationToken);
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine("Consumer cancelado.");
            }
        }
    }
}
