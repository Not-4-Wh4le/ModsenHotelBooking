using Application.Common.Interfaces.Repositories;
using Domain.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Persistence.Repositories
{
    public class MongoBookingRepository(MongoDbContext context)
        : MongoRepositoryBase<Booking>(context, MongoCollectionNames.Bookings), IBookingRepository
    {
        public async Task<(IReadOnlyList<Booking> Items, int TotalCount)> GetByHotelIdPagedAsync(
            Guid hotelId, 
            int page, 
            int pageSize, 
            CancellationToken cancellationToken)
        {
            var rooms = context.GetCollection<Room>(MongoCollectionNames.Rooms);
            var roomIds = await rooms.Find(r => r.HotelId == hotelId)
                .Project(r => r.Id)
                .ToListAsync(cancellationToken);

            if (roomIds.Count == 0)
                return (new List<Booking>(), 0);

            var filter = Builders<Booking>.Filter.In(b => b.RoomId, roomIds);

            var totalCount = await Collection.CountDocumentsAsync(filter, null, cancellationToken);

            var sort = Builders<Booking>.Sort.Descending(b => b.CheckInDate);

            var items = await Collection.Find(filter)
                .Sort(sort)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync(cancellationToken);

            return (items, (int)totalCount);
        }

        public async Task<(IReadOnlyList<Booking> Items, int TotalCount)> GetByUserIdPagedAsync(
            Guid userId, 
            int page, 
            int pageSize, 
            CancellationToken cancellationToken)
        {
            var totalCount = await Collection.Find(b => b.UserId == userId)
                .CountDocumentsAsync(cancellationToken);

            var sort = Builders<Booking>.Sort.Descending(b => b.CheckInDate);

            var items = await Collection.Find(b => b.UserId == userId)
                .Sort(sort)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync(cancellationToken);

            return (items, (int)totalCount);
        }

        public async Task<bool> HasOverlappingBookingsAsync(
            Guid roomId, 
            DateTimeOffset checkIn, 
            DateTimeOffset checkOut, 
            CancellationToken cancellationToken)
        {
            var hasOverlap = await Collection.Find(b =>
                b.RoomId == roomId &&
                b.BookingStatus != Domain.Enums.BookingStatus.Cancelled &&
                b.CheckInDate < checkOut &&
                b.CheckOutDate > checkIn)
                .AnyAsync(cancellationToken);

            return hasOverlap;
        }
    }
}
