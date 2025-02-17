using JobProcessor.Application.Commands;
using JobProcessor.Application.Ports;
using JobProcessor.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace JobProcessor.Application.Handlers
{
    public class CreateJobHandler : IRequestHandler<CreateJobCommand>
    {
        private readonly IJobRepository _jobRepository;
        private readonly IMessageQueue _messageQueue;
        private readonly ILogger<CreateJobHandler> _logger;
        private readonly IConfiguration _configuration;

        public CreateJobHandler(IJobRepository jobRepository, IMessageQueue messageQueuePublisher, ILogger<CreateJobHandler> logger, IConfiguration configuration)
        {
            _jobRepository = jobRepository;
            _messageQueue = messageQueuePublisher;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task Handle(CreateJobCommand command, CancellationToken cancellationToken)
        {
            if (command == null)
            {
                throw new ArgumentException("Command cannot be null.", nameof(command));
            }

            try
            {
                // Validações do comando
                if (string.IsNullOrWhiteSpace(command.JobType))
                {
                    throw new ArgumentException("Job type is required.", nameof(command.JobType));
                }

                if (command.Data == null)
                {
                    throw new ArgumentException("Job data is required.", nameof(command.Data));
                }

                _logger.LogInformation("Processing job creation...");

                // Criação do Job a partir do comando
                var job = new Job(command.JobType, command.Data);

                // Persistência do Job no repositório
                await _jobRepository.AddAsync(job, cancellationToken);

                // Publicação do Job na fila para processamento assíncrono
                var serializedJob = JsonConvert.SerializeObject(job);

                await _messageQueue.PublishAsync(serializedJob, cancellationToken);

                _logger.LogInformation($"Job created and published successfully. Job Id: {job.Id}");
            }
            catch (ArgumentException ex)
            {
                // Tratamento de erros de argumentos inválidos
                _logger.LogError(ex, "Invalid command parameters.");
                throw new ApplicationException("Invalid job creation parameters.", ex);
            }
            catch (Exception ex)
            {
                // Tratamento de falhas genéricas
                _logger.LogError(ex, "An error occurred while processing the job creation.");
                throw new ApplicationException("An unexpected error occurred during job creation.", ex);
            }
        }
    }
}
