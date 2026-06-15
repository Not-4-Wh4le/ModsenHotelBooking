using Application.Common.Interfaces;
using Application.Common.Interfaces.Repositories;
using Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Features.Rooms.Commands.UpdateRoom
{
    public class UpdateRoomCommandHandler(
        IRoomRepository roomRepository,
        IHotelRepository hotelRepository,
        ICurrentUserService currentUser)
        : IRequestHandler<UpdateRoomCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(UpdateRoomCommand request, CancellationToken cancellationToken)
        {
            var room = await roomRepository.GetByIdAsync(request.Id, cancellationToken);
            if (room == null)
                return Result<Guid>.Failure("Room not found");
            
            if(!room.IsDeleted)
                return Result<Guid>.Failure("Cannot update a deleted room");

            if (currentUser.Role == nameof(UserRole.HotelManager))
            {
                var hotel = await hotelRepository.GetByIdAsync(room.HotelId, cancellationToken);
                if (hotel == null)
                    return Result<Guid>.Failure("Hotel for this room does not exist");
             
                if (currentUser.Id != hotel.ManagerId)
                    return Result<Guid>.Failure("You cannot update a room in a hotel you do not manage");
            }

            try
            {
                var roomType = Enum.Parse<RoomType>(request.RoomType, ignoreCase: true);

                room.ChangeNumber(request.Number);
                room.ChangeRoomType(roomType);
                room.ChangeCapacity(request.Capacity);
                room.ChangePricePerNight(request.PricePerNight);
                room.ChangeDescription(request.Description);
                room.ChangeAmenities(request.Amenities);
            }
            catch (ArgumentException ex)
            {
                return Result<Guid>.Failure(ex.Message);
            }

            roomRepository.Update(room);
            return Result<Guid>.Success(room.Id);
        }
    }
}
