using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Features.Hotels.Queries.SearchHotels
{
    public record HotelDto(
        Guid Id,
        string Name,
        string City,
        string Country,
        string Address,
        double Rating
        );
}
