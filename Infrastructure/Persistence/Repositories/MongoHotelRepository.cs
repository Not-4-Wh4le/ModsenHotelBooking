using Application.Common.Interfaces.Repositories;
using Domain.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Persistence.Repositories
{
    public class MongoHotelRepository(MongoDbContext context)
        : MongoRepositoryBase<Hotel>(context, MongoCollectionNames.Hotels), IHotelRepository
    {
        private const string sortByName = "name";
        private const string sortByRating = "rating";
        public async Task<(IReadOnlyList<Hotel> Items, int TotalCount)> GetManagedAsync(
            Guid? managerId, 
            double? minRating, 
            int page, 
            int pageSize, 
            string sortBy, 
            bool isDescending, 
            CancellationToken cancellationToken)
        {
            var builder = Builders<Hotel>.Filter;
            var filter = builder.Empty;

            if (managerId.HasValue && managerId != Guid.Empty)
                filter &= builder.Eq(h => h.ManagerId, managerId);

            if (minRating.HasValue)
                filter &= builder.Gte(h => h.Rating, minRating);

            var sort = GetSortDefinition(sortBy, isDescending);

            var totalCount = await Collection.CountDocumentsAsync(filter, null, cancellationToken);
            
            var items = await Collection.Find(filter)
                .Sort(sort)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync(cancellationToken);

            return (items, (int)totalCount);
        }

        public async Task<bool> IsNameUniqueInCityAsync(string name, string city, CancellationToken cancellationToken)
        {
            var exist = await Collection.Find(
                h => h.Name == name && h.City == city)
                .AnyAsync(cancellationToken);

            return !exist;
        }

        public async Task<(IReadOnlyList<Hotel> Items, int TotalCount)> SearchAsync(
            string? city, 
            string? country, 
            double? minRating, 
            int page, 
            int pageSize, 
            string sortBy, 
            bool isDescending, 
            CancellationToken cancellationToken)
        {
            var builder = Builders<Hotel>.Filter;
            var filter = builder.Empty;

            if (!string.IsNullOrEmpty(city))
                filter &= builder.Eq(h => h.City, city);

            if (!string.IsNullOrEmpty(country))
                filter &= builder.Eq(h => h.Country, country);

            var sort = GetSortDefinition(sortBy, isDescending);

            var totalCount = await Collection.CountDocumentsAsync(filter, null, cancellationToken);
            var items = await Collection.Find(filter)
                .Sort(sort)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync(cancellationToken);

            return (items, (int)totalCount);
        }

        private SortDefinition<Hotel> GetSortDefinition(string? sortBy, bool isDescending)
        {
            var sortBuilder = Builders<Hotel>.Sort;

            return sortBy?.ToLower() switch
            {
                sortByName => isDescending ? sortBuilder.Descending(h => h.Name) : sortBuilder.Ascending(h => h.Name),
                sortByRating => isDescending ? sortBuilder.Descending(h => h.Rating) : sortBuilder.Ascending(h => h.Rating),
                _ => isDescending ? sortBuilder.Descending(h => h.Id) : sortBuilder.Ascending(h => h.Id)
            };
        }

    }
}
