using Application.Common.Interfaces;
using Application.Common.Interfaces.Repositories;
using Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Features.Bookings.Queries.GetHotelsBooking
{
    public class GetHotelsBookingQueryHandler(
        IHotelRepository hotelRepository,
        IBookingRepository bookingRepository,
        IUserRepository userRepository,
        IRoomRepository roomRepository,
        ICurrentUserService currentUser)
        : IRequestHandler<GetHotelsBookingQuery, Result<PagedResultDto<ManagerBookingDto>>>
    {
        public async Task<Result<PagedResultDto<ManagerBookingDto>>> Handle(GetHotelsBookingQuery request, CancellationToken cancellationToken)
        {
            var hotel = await hotelRepository.GetByIdAsync(request.HotelId, cancellationToken);
            if (hotel == null)
                return Result<PagedResultDto<ManagerBookingDto>>.Failure("Hotel not found");

            if(currentUser.Id != hotel.ManagerId)
                return Result<PagedResultDto<ManagerBookingDto>>.Failure("Forbbiden: You cannot view bookings of a hotel you do not manage");

            var (bookings, totalCount) = await bookingRepository.GetByHotelIdPagedAsync(
                request.HotelId, request.Page, request.PageSize, cancellationToken);
            
            var usersIds = bookings.Select(b => b.UserId).Distinct().ToList();
            var users = await userRepository.GetByIdsAsync(usersIds, cancellationToken);
            var usersDict = users.ToDictionary(u => u.Id);

            var roomsIds = bookings.Select(b => b.RoomId).Distinct().ToList();
            var rooms = await roomRepository.GetByIdsAsync(roomsIds, cancellationToken);
            var roomsDict = rooms.ToDictionary(r => r.Id);

            var dtos = bookings.Select(booking =>
            {
                var userFound = usersDict.TryGetValue(booking.UserId, out var user);
                var roomFound = roomsDict.TryGetValue(booking.RoomId, out var room);
                return new ManagerBookingDto(
                    booking.Id,
                    userFound ? user!.Username : "Unknown user",
                    userFound ? user!.Email : "Unknown user",
                    roomFound ? room!.Number : 0,
                    booking.CheckInDate,
                    booking.CheckOutDate,
                    booking.FinalPrice,
                    booking.BookingStatus.ToString());
            }).ToList();

            var result = new PagedResultDto<ManagerBookingDto>(
                dtos, totalCount, request.Page, request.PageSize);
            return Result<PagedResultDto<ManagerBookingDto>>.Success(result);
        }
    }
}
