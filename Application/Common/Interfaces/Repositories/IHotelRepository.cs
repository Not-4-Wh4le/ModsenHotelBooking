using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Interfaces.Repositories
{
    public interface IHotelRepository : IRepositoryBase<Hotel>
    {
        Task<bool> IsNameUniqueInCityAsync(string name, string city, CancellationToken cancellationToken);
        Task<(IReadOnlyList<Hotel> Items, int TotalCount)> SearchAsync(
            string? city,
            string? country,
            decimal? minRating,
            int page, 
            int pageSize,
            string sortBy,
            bool isDesceding,
            CancellationToken cancellationToken);
    }
}
