using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Features.Rooms.Queries.SearchRooms
{
    public record RoomCatalogDto(
        Guid Id,
        Guid HotelId,
        string HotelName,
        string HotelCity,
        string Type,
        int Capacity,
        decimal PricePerNight,
        string Amenities);
}
