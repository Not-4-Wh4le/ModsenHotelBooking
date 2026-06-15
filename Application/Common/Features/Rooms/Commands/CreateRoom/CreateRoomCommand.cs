using Application.Common.Security;
using Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Features.Rooms.Commands.CreateRoom
{
    [Authorize(UserRole.Admin, UserRole.HotelManager)]
    public record CreateRoomCommand(
        int Number,
        Guid HotelId,
        string RoomType,
        int Capacity,
        decimal PricePerNight,
        string Description,
        string Amenities,
        int Area)
        : IRequest<Result<Guid>>;
}
