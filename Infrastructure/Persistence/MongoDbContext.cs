using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
namespace Infrastructure.Persistence
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase database;

        public MongoDbContext(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("MongoDb")
                ?? throw new ArgumentNullException("Connection string 'MongoDb' is missing");

            var mongoUrl = new MongoUrl(connectionString);

            var mongoClient = new MongoClient(mongoUrl);

            var databaseName = configuration["DatabaseSettings:DatabaseName"];

            database = mongoClient.GetDatabase(databaseName);
        }

        public IMongoCollection<T> GetCollection<T>(string name) => database.GetCollection<T>(name);
    }
}
