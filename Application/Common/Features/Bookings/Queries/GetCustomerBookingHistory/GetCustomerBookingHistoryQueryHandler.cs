using Application.Common.Interfaces;
using Application.Common.Interfaces.Repositories;
using Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Features.Bookings.Queries.GetCustomerBookingHistory
{
    public class GetCustomerBookingHistoryQueryHandler(
        IBookingRepository bookingRepository,
        ICurrentUserService currentUser,
        IRoomRepository roomRepository,
        IHotelRepository hotelRepository
        )
        : IRequestHandler<GetCustomerBookingHistoryQuery, Result<PagedResultDto<CustomerBookingHistoryDto>>>
    {
        public async Task<Result<PagedResultDto<CustomerBookingHistoryDto>>> Handle(GetCustomerBookingHistoryQuery request, CancellationToken cancellationToken)
        {
            if(currentUser.Id != request.CustomerId)
                return Result<PagedResultDto<CustomerBookingHistoryDto>>.Failure("Forbidden: You can only view your own booking history");

            var (bookings, totalCount) = await bookingRepository.GetByUserIdPagedAsync(
                request.CustomerId,
                request.Page,
                request.PageSize,
                cancellationToken);

            var roomIds = bookings.Select(b => b.RoomId).Distinct().ToList();

            var rooms = await roomRepository.GetByIdsAsync(roomIds, cancellationToken);
            var roomsDict = rooms.ToDictionary(r => r.Id);


            var hotelIds = rooms.Select(r => r.HotelId).Distinct().ToList();
            var hotels = await hotelRepository.GetByIdsAsync(hotelIds, cancellationToken);
            var hotelsDict = hotels.ToDictionary(h => h.Id);
            var dtos = bookings.Select(booking =>
            {
                var roomFound = roomsDict.TryGetValue(booking.RoomId, out var room);
                var hotelFound = hotelsDict.TryGetValue(room.HotelId, out var hotel);
                return new CustomerBookingHistoryDto(
                    booking.Id,
                    hotelFound ? hotel!.Name : "Unknown hotel",
                    roomFound ? room.RoomType.ToString() : "Unknown type",
                    roomFound ? room.Number : 0,
                    booking.CheckInDate,
                    booking.CheckOutDate,
                    booking.FinalPrice,
                    booking.BookingStatus.ToString());
            }).ToList();

            var result = new PagedResultDto<CustomerBookingHistoryDto>(dtos, totalCount, request.Page, request.PageSize);
            return Result<PagedResultDto<CustomerBookingHistoryDto>>.Success(result);
        }
    }
}
