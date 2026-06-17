using Application.Common.Interfaces.Repositories;
using Domain.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Persistence.Repositories
{
    public class MongoLoyaltyRepository(MongoDbContext context)
        : MongoRepositoryBase<LoyaltyProgram>(context, MongoCollectionNames.LoyaltyPrograms), ILoyaltyRepository
    {
        public async Task CreateOrUpdateAsync(LoyaltyProgram loyaltyProgram, CancellationToken cancellationToken)
        {
            var filter = Builders<LoyaltyProgram>.Filter.Eq(f => f.Id, loyaltyProgram.Id);
            var options = new ReplaceOptions { IsUpsert = true };
            await Collection.ReplaceOneAsync(
                filter, loyaltyProgram, options, cancellationToken);            
        }

        public async Task<LoyaltyProgram?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await Collection.Find(l => l.UserId == userId).FirstOrDefaultAsync(cancellationToken);
        }
    }
}
