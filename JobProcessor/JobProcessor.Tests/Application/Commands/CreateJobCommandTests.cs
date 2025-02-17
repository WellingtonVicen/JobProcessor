using JobProcessor.Application.Commands;

namespace JobProcessor.Tests.Application.Commands
{
    public class CreateJobCommandTests
    {
        [Fact]
        public void CreateJobCommand_ShouldThrowException_WhenJobTypeIsNullOrEmpty()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new CreateJobCommand(null!, "Some data"));
            Assert.Equal("Job type cannot be null or empty. (Parameter 'jobType')", exception.Message);

            exception = Assert.Throws<ArgumentException>(() => new CreateJobCommand("", "Some data"));
            Assert.Equal("Job type cannot be null or empty. (Parameter 'jobType')", exception.Message);
        }

        [Fact]
        public void CreateJobCommand_ShouldThrowException_WhenDataIsNullOrEmpty()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new CreateJobCommand("SendEmail", null!));
            Assert.Equal("Job data cannot be null or empty. (Parameter 'data')", exception.Message);

            exception = Assert.Throws<ArgumentException>(() => new CreateJobCommand("SendEmail", ""));
            Assert.Equal("Job data cannot be null or empty. (Parameter 'data')", exception.Message);
        }

        [Fact]
        public void CreateJobCommand_ShouldCreateCommand_WhenValidParametersArePassed()
        {
            // Act
            var command = new CreateJobCommand("SendEmail", "{\"email\":\"test@example.com\"}");

            // Assert
            Assert.Equal("SendEmail", command.JobType);
            Assert.Equal("{\"email\":\"test@example.com\"}", command.Data);
        }
    }
}
