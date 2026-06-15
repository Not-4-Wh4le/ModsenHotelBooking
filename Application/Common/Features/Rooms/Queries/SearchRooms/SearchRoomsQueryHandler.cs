using Application.Common.Interfaces.Repositories;
using Application.Common.Models;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Features.Rooms.Queries.SearchRooms
{
    public class SearchRoomsQueryHandler(
        IRoomRepository roomRepository,
        IHotelRepository hotelRepository)
        : IRequestHandler<SearchRoomsQuery, Result<PagedResultDto<RoomCatalogDto>>>
    {
        public async Task<Result<PagedResultDto<RoomCatalogDto>>> Handle(SearchRoomsQuery request, CancellationToken cancellationToken)
        {
            var (rooms, totalCount) = await roomRepository.GetFilteredCatalogAsync(
                request.HotelId,
                request.Type,
                request.Capacity,
                request.MinPrice,
                request.MaxPrice,
                request.AmenitySearch,
                request.CheckIn,
                request.CheckOut,
                request.Page,
                request.PageSize,
                request.SortBy,
                request.IsDescending,
                cancellationToken);

            var hotelIds = rooms.Select(r => r.HotelId).Distinct();
            var hotels = await hotelRepository.GetByIdsAsync(hotelIds, cancellationToken);
            var hotelsDict = hotels.ToDictionary(h => h.Id);
            var dtos = rooms.Select(r =>
            {
                var hotelFound = hotelsDict.TryGetValue(r.HotelId, out var hotel);
                return new RoomCatalogDto(
                 r.Id,
                 r.HotelId,
                 hotelFound ? hotel!.Name : "Unknown Hotel",
                 hotelFound ? hotel!.City : "Unknown City",
                 r.RoomType.ToString(),
                 r.Capacity,
                 r.PricePerNight,
                 r.Amenities
             );
            }).ToList();
            var result = new PagedResultDto<RoomCatalogDto>(dtos, totalCount, request.Page, request.PageSize);
            return Result<PagedResultDto<RoomCatalogDto>>.Success(result);
        }
    }
}
