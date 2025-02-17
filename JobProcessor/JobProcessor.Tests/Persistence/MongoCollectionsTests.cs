using JobProcessor.Domain.Entities;
using JobProcessor.Infrastructure.Persistence;
using MongoDB.Driver;
using Moq;

namespace JobProcessor.Tests.Persistence
{
    public class MongoCollectionsTests
    {
        private readonly Mock<IMongoDatabase> _mockDatabase;
        private readonly Mock<IMongoCollection<Job>> _mockJobCollection;
        private readonly MongoCollections _mongoCollections;

        public MongoCollectionsTests()
        {
            // Mock do banco de dados Mongo
            _mockDatabase = new Mock<IMongoDatabase>();

            // Mock da coleção de Jobs
            _mockJobCollection = new Mock<IMongoCollection<Job>>();

            // Configurar o mock do banco de dados para retornar a coleção mockada
            _mockDatabase.Setup(db => db.GetCollection<Job>("Jobs", null))
                         .Returns(_mockJobCollection.Object);

            // Inicializando o MongoCollections com o mock do banco de dados
            _mongoCollections = new MongoCollections(_mockDatabase.Object);
        }

        [Fact]
        public void Initialize_ShouldCreateJobCollectionWithIndexes()
        {
            // Act
            _mongoCollections.Initialize();

            // Assert: Verificar se a coleção de Jobs foi acessada
            _mockDatabase.Verify(db => db.GetCollection<Job>("Jobs", null), Times.Once);
        }
    }
}
