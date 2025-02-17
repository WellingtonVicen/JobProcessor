using JobProcessor.Application.Ports;
using JobProcessor.Domain.Entities;
using JobProcessor.Domain.Enums;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace JobProcessor.Infrastructure.Persistence
{
    public class JobRepository : IJobRepository
    {
        private readonly IMongoCollection<Job> _jobCollection;
        private readonly ILogger<JobRepository> _logger;

        public JobRepository(IMongoCollection<Job> jobCollection, ILogger<JobRepository> logger)
        {
            _jobCollection = jobCollection ?? throw new ArgumentNullException(nameof(jobCollection));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task AddAsync(Job job, CancellationToken cancellationToken)
        {
            try
            {
                await _jobCollection.InsertOneAsync(job, null, cancellationToken);
            }
            catch (Exception ex)
            {
                // Log o erro e levante uma exceção com informações claras
                throw new ApplicationException("An error occurred while adding the job.", ex);
            }
        }

        public async Task<Job> GetByIdAsync(Guid jobId, CancellationToken cancellationToken)
        {
            try
            {
                var job = await _jobCollection.Find(job => job.Id == jobId)
                    .FirstOrDefaultAsync(cancellationToken);

                return job is null ? throw new KeyNotFoundException($"Job with ID {jobId} not found.") : job;
            }
            catch (Exception ex)
            {
                // Log o erro e levante uma exceção
                throw new ApplicationException("An error occurred while retrieving the job.", ex);
            }
        }

        public async Task<IEnumerable<Job>> GetAllPendingJobsAsync(CancellationToken cancellationToken)
        {
            try
            {
                // Filtro para buscar jobs com status "Pendente"
                var filter = Builders<Job>.Filter.Eq(job => job.Status, JobStatus.Pending);

                var jobs = await _jobCollection.Find(filter)
                    .ToListAsync(cancellationToken);

                return jobs;
            }
            catch (Exception ex)
            {
                // Log o erro e levante uma exceção
                throw new ApplicationException("An error occurred while retrieving pending jobs.", ex);
            }
        }

        public async Task UpdateAsync(Job job, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _jobCollection.ReplaceOneAsync(
                    j => j.Id == job.Id,
                    job,
                    new ReplaceOptions { IsUpsert = false },
                    cancellationToken);

                if (result.MatchedCount == 0)
                {
                    throw new KeyNotFoundException($"Job with ID {job.Id} not found.");
                }
            }
            catch (Exception ex)
            {
                // Log o erro e levante uma exceção
                throw new ApplicationException("An error occurred while updating the job.", ex);
            }
        }

        public async Task DeleteAsync(Guid jobId, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _jobCollection.DeleteOneAsync(
                    job => job.Id == jobId,
                    cancellationToken);

                if (result.DeletedCount == 0)
                {
                    throw new KeyNotFoundException($"Job with ID {jobId} not found.");
                }
            }
            catch (Exception ex)
            {
                // Log o erro e levante uma exceção
                throw new ApplicationException("An error occurred while deleting the job.", ex);
            }
        }
    }
}
