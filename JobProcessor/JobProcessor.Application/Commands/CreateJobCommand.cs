using MediatR;

namespace JobProcessor.Application.Commands
{
    public class CreateJobCommand : IRequest
    {
        public string? JobType { get; private set; }
        public string? Data { get; private set; }

        public CreateJobCommand(string jobType, string data)
        {
            if (string.IsNullOrEmpty(jobType))
                throw new ArgumentException("Job type cannot be null or empty.", nameof(jobType));

            if (string.IsNullOrEmpty(data))
                throw new ArgumentException("Job data cannot be null or empty.", nameof(data));

            JobType = jobType;
            Data = data;
        }
    }
}
