using JobProcessor.Domain.Entities;
using JobProcessor.Domain.Enums;

namespace JobProcessor.Tests.Domain
{
    public class JobTests
    {
        [Fact]
        public void Job_Creation_Should_Set_Status_To_Pending()
        {
            // Arrange
            var job = new Job("EnviarEmail", "{}");

            // Assert
            Assert.Equal(JobStatus.Pending, job.Status);
        }

        [Fact]
        public void Job_UpdateStatus_Should_Update_Status()
        {
            // Arrange
            var job = new Job("GerarRelatorio", "{}");

            // Act
            job.UpdateStatus(JobStatus.InProcess);

            // Assert
            Assert.Equal(JobStatus.InProcess, job.Status);
        }

        [Fact]
        public void Job_IncrementRetryCount_Should_Increment_RetryCount()
        {
            // Arrange
            var job = new Job("GerarRelatorio", "{}");

            // Act
            job.IncrementRetryCount();

            // Assert
            Assert.Equal(1, job.RetryCount);

            // Act
            job.IncrementRetryCount();

            // Assert
            Assert.Equal(2, job.RetryCount);
        }

        [Fact]
        public void Job_IncrementRetryCount_Should_Throw_Exception_When_Max_Retries_Reached()
        {
            // Arrange
            var job = new Job("GerarRelatorio", "{}");

            // Act
            job.IncrementRetryCount();
            job.IncrementRetryCount();
            job.IncrementRetryCount();

            // Assert
            var exception = Assert.Throws<InvalidOperationException>(() => job.IncrementRetryCount());
            Assert.Equal("Máximo de tentativas alcançado.", exception.Message);
        }

        [Fact]
        public void Job_CanRetry_Should_Return_False_After_Max_Retries()
        {
            // Arrange
            var job = new Job("GerarRelatorio", "{}");
            job.IncrementRetryCount();
            job.IncrementRetryCount();
            job.IncrementRetryCount();

            // Assert
            Assert.False(job.CanRetry());
        }
    }
}
