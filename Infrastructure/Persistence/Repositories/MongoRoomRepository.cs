using Application.Common.Interfaces.Repositories;
using Domain.Entities;
using Domain.Enums;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Persistence.Repositories
{
    public class MongoRoomRepository(MongoDbContext context)
        : MongoRepositoryBase<Room>(context, MongoCollectionNames.Rooms), IRoomRepository
    {
        private const string sortByPrice = "price";
        private const string sortByCapacity = "capacity";
        public async Task<(IReadOnlyList<Room> Items, int TotalCount)> GetFilteredCatalogAsync(
            Guid? hotelId,
            string? type, 
            int? capacity, 
            decimal? minPrice, 
            decimal? maxPrice, 
            string? amenitySearch, 
            DateTimeOffset? checkIn, 
            DateTimeOffset? checkOut, 
            int page, 
            int pageSize, 
            string sortBy, 
            bool isDescending, 
            CancellationToken cancellationToken)
        {
            var builder = Builders<Room>.Filter;
            var filter = builder.Empty;

            if (hotelId.HasValue && hotelId.Value != Guid.Empty)
                filter &= builder.Eq(r => r.HotelId, hotelId);

            if(!string.IsNullOrWhiteSpace(type) && Enum.TryParse<RoomType>(type, true, out var roomType))
                filter &= builder.Eq(r => r.RoomType, roomType);

            if (capacity.HasValue)
                filter &= builder.Gte(r => r.Capacity, capacity);

            if (minPrice.HasValue)
                filter &= builder.Gte(r => r.PricePerNight, minPrice);

            if (maxPrice.HasValue)
                filter &= builder.Lte(r => r.PricePerNight, maxPrice);

            if (!string.IsNullOrEmpty(amenitySearch))
                filter &= builder.Regex(r => r.Amenities, new BsonRegularExpression(amenitySearch, "i"));

            if(checkIn.HasValue && checkOut.HasValue)
            {
                var bookingCollection = context.GetCollection<Booking>(MongoCollectionNames.Bookings);

                var overlapFilter = Builders<Booking>.Filter.And(
                    Builders<Booking>.Filter.Lt(b => b.CheckInDate, checkOut),
                    Builders<Booking>.Filter.Gt(b => b.CheckOutDate, checkIn),
                    Builders<Booking>.Filter.Ne(b => b.BookingStatus, BookingStatus.Cancelled));

                var bookedRoomIds = await bookingCollection.Find(overlapFilter)
                    .Project(b => b.RoomId)
                    .ToListAsync(cancellationToken);

                if (bookedRoomIds.Count != 0)
                    filter &= builder.Nin(r => r.Id, bookedRoomIds);
            }
            var sortBuilder = Builders<Room>.Sort;
            var sort = sortBy.ToLower() switch
            {
                sortByPrice => isDescending ? sortBuilder.Descending(r => r.PricePerNight) : sortBuilder.Ascending(r => r.PricePerNight),
                sortByCapacity => isDescending ? sortBuilder.Descending(r => r.Capacity) : sortBuilder.Ascending(r => r.Capacity),
                _ => isDescending ? sortBuilder.Descending(r => r.Id) : sortBuilder.Ascending(r => r.Id)
            };

            var totalCount = await Collection.CountDocumentsAsync(filter, null, cancellationToken);
            var items = await Collection.Find(filter)
                .Sort(sort)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync(cancellationToken);

            return (items, (int)totalCount);
        }
    }
}
