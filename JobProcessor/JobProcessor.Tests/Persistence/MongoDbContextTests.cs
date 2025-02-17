using FluentAssertions;
using JobProcessor.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Moq;

namespace JobProcessor.Tests.Persistence
{
    public class MongoDbContextTests
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly MongoDbContext _mongoDbContext;

        public MongoDbContextTests()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockConfiguration.Setup(config => config.GetConnectionString("MongoDb")).Returns("mongodb://localhost:27017");

            _mongoDbContext = new MongoDbContext(_mockConfiguration.Object);
        }

        [Fact]
        public void MongoDbContext_ShouldInitializeDatabase_WhenValidConnectionString()
        {
            // Act & Assert
            var jobsCollection = _mongoDbContext.Jobs;
            jobsCollection.Should().NotBeNull();
        }

        [Fact]
        public void MongoDbContext_ShouldThrowArgumentException_WhenConnectionStringIsMissing()
        {
            // Arrange
            _mockConfiguration.Setup(config => config.GetConnectionString("MongoDb")).Returns(string.Empty);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => new MongoDbContext(_mockConfiguration.Object));
        }
    }
}
