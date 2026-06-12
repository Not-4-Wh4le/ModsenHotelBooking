using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Interfaces.Repositories
{
    public interface IBookingRepository : IRepositoryBase<Booking>
    {
        Task<(IReadOnlyList<Booking> Items, int TotalCount)> GetByUserIdPagedAsync(
            Guid userId,
            int page,
            int pageSize,
            CancellationToken cancellationToken);
        Task<(IReadOnlyList<Booking> Items, int TotalCount)> GetByHotelIdAsync(
            Guid hotelId,
            int page,
            int pageSize,
            CancellationToken cancellationToken);

        Task<bool> HasOverlappingBookingsAsync(
            Guid roomId,
            DateTimeOffset checkIn,
            DateTimeOffset checkOut,
            CancellationToken cancellationToken);
    }
}
