using FluentAssertions;
using JobProcessor.Application.Commands;
using JobProcessor.Application.Handlers;
using JobProcessor.Application.Ports;
using JobProcessor.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace JobProcessor.Tests.Application.Handles
{
    public class CreateJobHandlerTests
    {
        private readonly Mock<IJobRepository> _mockJobRepository;
        private readonly Mock<IMessageQueuePublisher> _mockMessageQueuePublisher;
        private readonly Mock<ILogger<CreateJobHandler>> _mockLogger;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly CreateJobHandler _handler;

        public CreateJobHandlerTests()
        {
            _mockJobRepository = new Mock<IJobRepository>();
            _mockMessageQueuePublisher = new Mock<IMessageQueuePublisher>();
            _mockLogger = new Mock<ILogger<CreateJobHandler>>();
            _configurationMock = new Mock<IConfiguration>();
            _handler = new CreateJobHandler(_mockJobRepository.Object, _mockMessageQueuePublisher.Object, _mockLogger.Object, _configurationMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldThrowArgumentException_WhenCommandIsNull()
        {
            // Arrange
            CreateJobCommand? command = null;

            // Act
            Func<Task> act = async () => await _handler.Handle(command!, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Command cannot be null.*");
        }

        [Fact]
        public async Task Handle_ShouldThrowArgumentException_WhenJobTypeIsEmpty()
        {
            // Arrange
            var command = new CreateJobCommand("", "Job Data");

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Job type is required.");
        }

        [Fact]
        public async Task Handle_ShouldCreateAndPublishJobSuccessfully()
        {
            // Arrange
            var command = new CreateJobCommand("SendEmail","Job Data");
            var job = new Job(command.JobType!, command.Data!);

            _mockJobRepository.Setup(repo => repo.AddAsync(It.IsAny<Job>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _mockMessageQueuePublisher.Setup(publisher => publisher.PublishAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().NotThrowAsync();
            _mockJobRepository.Verify(repo => repo.AddAsync(It.Is<Job>(j => j.JobType == "SendEmail" && j.Data == "Job Data"), It.IsAny<CancellationToken>()), Times.Once);
            _mockMessageQueuePublisher.Verify(publisher => publisher.PublishAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        public async Task Handle_ShouldLogError_WhenJobRepositoryFails()
        {
            // Arrange
            var command = new CreateJobCommand("SendEmail","Job Data");

            _mockJobRepository.Setup(repo => repo.AddAsync(It.IsAny<Job>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ApplicationException>()
                .WithMessage("An unexpected error occurred during job creation.");
            _mockLogger.Verify(log => log.LogError(It.IsAny<Exception>(), "An error occurred while processing the job creation."), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldLogError_WhenMessageQueueFails()
        {
            // Arrange
            var command = new CreateJobCommand("SendEmail","Job Data");
            var job = new Job(command.JobType!, command.Data!);

            _mockJobRepository.Setup(repo => repo.AddAsync(It.IsAny<Job>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _mockMessageQueuePublisher.Setup(publisher => publisher.PublishAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Queue error"));

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ApplicationException>()
                .WithMessage("An unexpected error occurred during job creation.");
            _mockLogger.Verify(log => log.LogError(It.IsAny<Exception>(), "An error occurred while processing the job creation."), Times.Once);
        }
    }
}
