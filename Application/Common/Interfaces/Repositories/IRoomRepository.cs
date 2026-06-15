using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Interfaces.Repositories
{
    public interface IRoomRepository : IRepositoryBase<Room>
    {
        Task<(IReadOnlyList<Room> Items, int TotalCount)> GetFilteredCatalogAsync(
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
            CancellationToken cancellationToken);
    }
}
