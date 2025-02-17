using JobProcessor.Domain.Entities;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace JobProcessor.Infrastructure.Persistence
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;
        private readonly MongoCollections _mongoCollections;

        public MongoDbContext(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("MongoDb");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentException("MongoDB connection string is missing.");
            }

            // Conecta ao banco de dados MongoDB
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(configuration["MongoDbSettings:DatabaseName"]);

            // Inicializa as coleções usando a classe MongoCollections
            _mongoCollections = new MongoCollections(_database);
            _mongoCollections.Initialize();
        }

        public IMongoCollection<Job> Jobs => _database.GetCollection<Job>("Jobs");
    }
}
