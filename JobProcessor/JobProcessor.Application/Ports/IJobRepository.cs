using JobProcessor.Domain.Entities;
namespace JobProcessor.Application.Ports
{
    public interface IJobRepository
    {
        Task AddAsync(Job job, CancellationToken cancellationToken);
        Task<Job> GetByIdAsync(Guid jobId, CancellationToken cancellationToken);
        Task<IEnumerable<Job>> GetAllPendingJobsAsync(CancellationToken cancellationToken);
        Task UpdateAsync(Job job, CancellationToken cancellationToken);
        Task DeleteAsync(Guid jobId, CancellationToken cancellationToken);
    }
}
