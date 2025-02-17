using JobProcessor.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobProcessor.Domain.Entities
{
    public class Job
    {
        public Guid Id { get; private set; }
        public string? JobType { get; private set; }
        public string? Data { get; private set; }
        public JobStatus Status { get; private set; }
        public int RetryCount { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }


        public Job(string jobType, string data)
        {
            Id = Guid.NewGuid(); // Gerando o ID automaticamente
            JobType = jobType ?? throw new ArgumentNullException(nameof(jobType));
            Data = data ?? throw new ArgumentNullException(nameof(data));
            Status = JobStatus.Pending; // Status inicial é "Pendente"
            RetryCount = 0; // Inicialmente, sem tentativas
            CreatedAt = DateTime.UtcNow;
        }


        public void UpdateStatus(JobStatus status)
        {
            if (status == Status) return;

            Status = status;
            UpdatedAt = DateTime.UtcNow;
        }

        // Método para aumentar o contador de tentativas - Seguindo o princípio de **Abstração** (abstraindo a lógica do retry)
        public void IncrementRetryCount()
        {
            if (RetryCount >= 3)
                throw new InvalidOperationException("Máximo de tentativas alcançado.");

            RetryCount++;
        }

        // Método que verifica se a tarefa pode ser processada novamente
        public bool CanRetry() => RetryCount < 3 && Status == JobStatus.Error;

        // Método que retorna os dados da tarefa como JSON
        // Considerando que "Data" seja em formato JSON
        public string GetJobData() => Data!;

        // Método de exibição para depuração
        public override string ToString()
        {
            return $"Job {Id} - Tipo: {JobType}, Status: {Status}, Tentativas: {RetryCount}, Criado em: {CreatedAt}";
        }
    }
}