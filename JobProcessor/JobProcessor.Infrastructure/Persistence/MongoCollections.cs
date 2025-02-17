using JobProcessor.Domain.Entities;
using MongoDB.Driver;

namespace JobProcessor.Infrastructure.Persistence
{
    public class MongoCollections
    {
        private readonly IMongoDatabase _database;

        public MongoCollections(IMongoDatabase database)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
        }

        // Método para inicializar todas as coleções necessárias
        public void Initialize()
        {
            CreateJobsCollection();
        }

        // Método para configurar a coleção de Jobs
        private void CreateJobsCollection()
        {
            var collection = _database.GetCollection<Job>("Jobs");

            // Criar um índice para a propriedade 'JobType' para melhorar a busca
            var indexOptions = new CreateIndexOptions { Unique = false };
            var indexKeys = Builders<Job>.IndexKeys.Ascending(job => job.JobType);
            collection.Indexes.CreateOne(new CreateIndexModel<Job>(indexKeys, indexOptions));

            // Caso precise de algum outro índice ou configuração, pode adicionar aqui.
            // Exemplo de índice para buscar por 'Status' de maneira mais eficiente.
            var statusIndex = Builders<Job>.IndexKeys.Ascending(job => job.Status);
            collection.Indexes.CreateOne(new CreateIndexModel<Job>(statusIndex, indexOptions));

            // Adicionar mais coleções ou índices conforme necessário
        }
    }
}
