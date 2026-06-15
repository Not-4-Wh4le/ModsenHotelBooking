using Application.Common.Interfaces;
using Application.Common.Interfaces.Repositories;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Features.Rooms.Commands.CreateRoom
{
    public class CreateRoomCommandHandler(
        IRoomRepository roomRepository,
        ICurrentUserService currentUser,
        IHotelRepository hotelRepository)
        : IRequestHandler<CreateRoomCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(CreateRoomCommand request, CancellationToken cancellationToken)
        {
            var hotel = await hotelRepository.GetByIdAsync(request.HotelId, cancellationToken);

            if (hotel == null)
                return Result<Guid>.Failure("Hotel does not exist");

            if(currentUser.Role == nameof(UserRole.HotelManager) && currentUser.Id != hotel.ManagerId)
                return Result<Guid>.Failure("Forbidden: You can not add a room to a hotel you do not manage");
            Room room;
            try
            {
                var roomType = Enum.Parse<RoomType>(request.RoomType, ignoreCase: true);
                room = new Room(
                    Guid.NewGuid(),
                    request.Number,
                    request.HotelId,
                    roomType,
                    request.PricePerNight,
                    request.Capacity,
                    request.Description,
                    request.Amenities,
                    request.Area);
            }
            catch(ArgumentException ex)
            {
                return Result<Guid>.Failure(ex.Message);
            }

            await roomRepository.AddAsync(room, cancellationToken);
            return Result<Guid>.Success(room.Id);
        }
    }
}
