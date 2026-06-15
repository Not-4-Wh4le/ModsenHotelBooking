using Application.Common.Interfaces;
using Application.Common.Interfaces.Repositories;
using Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Features.Rooms.Commands.DeleteRoom
{
    public class DeleteRoomCommandHandler(
        IRoomRepository roomRepository,
        ICurrentUserService currentUser,
        IHotelRepository hotelRepository)
        : IRequestHandler<DeleteRoomCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(DeleteRoomCommand request, CancellationToken cancellationToken)
        {
            var room = await roomRepository.GetByIdAsync(request.Id, cancellationToken);

            if (room == null)
                return Result<Guid>.Failure("Room not found");

            if (currentUser.Role == nameof(UserRole.HotelManager))
            {
                var hotel = await hotelRepository.GetByIdAsync(room.HotelId, cancellationToken);
                if (hotel == null)
                    return Result<Guid>.Failure("Hotel for this room does not exist");
                
                if (currentUser.Id != hotel.ManagerId)
                    return Result<Guid>.Failure("Forbidden: You can not delete a room from a hotel you do not manage");
            }

            if (room.IsDeleted)
                return Result<Guid>.Failure("Room is already deleted");

            room.DeleteRoom();
            roomRepository.Update(room);
            return Result<Guid>.Success(room.Id);

        }
    }
}
