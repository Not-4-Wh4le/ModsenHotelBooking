using Application.Common.Interfaces;
using Domain.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Persistence.Repositories
{
    public class MongoUserRepository(MongoDbContext context)
        : MongoRepositoryBase<User>(context, MongoCollectionNames.Users), IUserRepository
    {
        public async Task<(IReadOnlyList<User> Items, int TotalCount)> GetPagedAsync(
            int page, int pageSize, CancellationToken cancellationToken)
        {
            var totalCount = await Collection.CountDocumentsAsync(Builders<User>.Filter.Empty, null, cancellationToken);
            var items = await Collection.Find(Builders<User>.Filter.Empty)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync(cancellationToken);
            return (items, (int)totalCount);
        }

        public async Task<User?> GetUserByEmailAsync(
            string email, CancellationToken cancellationToken)
        {
            return await Collection.Find(u => u.Email == email).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<User?> GetUserByUsernameAsync(
            string username, CancellationToken cancellationToken)
        {
            return await Collection.Find(u => u.Email == username).FirstOrDefaultAsync(cancellationToken);
        }
    }
}
