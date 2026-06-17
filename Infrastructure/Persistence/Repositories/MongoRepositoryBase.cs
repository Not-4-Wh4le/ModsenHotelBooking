using Application.Common.Interfaces.Repositories;
using Domain.Common;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Persistence.Repositories
{
    public abstract class MongoRepositoryBase<T> : IRepositoryBase<T> where T : class, IEntity
    {
        protected readonly IMongoCollection<T> Collection;
        protected MongoRepositoryBase(MongoDbContext context, string collectionName)
        {
            Collection = context.GetCollection<T>(collectionName);
        }
        public async Task AddAsync(T entity, CancellationToken cancellationToken)
        {
            await Collection.InsertOneAsync(entity, null, cancellationToken);
        }

        public void Delete(T entity)
        {
            var filter = Builders<T>.Filter.Eq(e => e.Id, entity.Id);

            Collection.DeleteOne(filter);
        }

        public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var filter = Builders<T>.Filter.Eq(e => e.Id, id);
            return await Collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<T>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken)
        {
            var filter = Builders<T>.Filter.In(e => e.Id, ids);
            return await Collection.Find(filter).ToListAsync(cancellationToken);
        }

        public void Update(T entity)
        {
            var filter = Builders<T>.Filter.Eq(e => e.Id, entity.Id);
            Collection.ReplaceOne(filter, entity);
        }
    }
}
