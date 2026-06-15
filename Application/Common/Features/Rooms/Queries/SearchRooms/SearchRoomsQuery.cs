using Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Features.Rooms.Queries.SearchRooms
{
    public record SearchRoomsQuery(
        Guid? HotelId = null,
        string? Type = null,
        int? Capacity = null,
        decimal MinPrice = 0,
        decimal? MaxPrice = null,
        string? AmenitySearch = null,
        DateTimeOffset? CheckIn =null,
        DateTimeOffset? CheckOut = null,
        int Page = 1,
        int PageSize = 10,
        string SortBy = "Price",
        bool IsDescending = false)
        : IRequest<Result<PagedResultDto<RoomCatalogDto>>>;
}
