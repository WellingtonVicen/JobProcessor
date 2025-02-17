using JobProcessor.Application.Ports;
using JobProcessor.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using RabbitMQ.Client;
using System.Reflection;

namespace JobProcessor.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddMediator(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
                Assembly.Load("JobProcessor.Application")
            ));
            return services;
        }


        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Configuração do MongoDB
            services.AddSingleton<IMongoClient>(sp =>
            {
                var connectionString = configuration.GetConnectionString("MongoDB");
                return new MongoClient(connectionString);
            });

            services.AddScoped<MongoDbContext>();
            services.AddScoped<IJobRepository, JobRepository>();


            //Config RabbitMQ

            //services.AddSingleton<IConnection>(sp =>
            //{
            //    var factory = new ConnectionFactory()
            //    {

            //    };

            //});
        }
    }
}
