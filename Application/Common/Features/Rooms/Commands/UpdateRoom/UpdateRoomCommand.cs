using Application.Common.Security;
using Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Features.Rooms.Commands.UpdateRoom
{
    [Authorize(UserRole.Admin, UserRole.HotelManager)]
    public record UpdateRoomCommand(
        Guid Id,
        int Number,
        string RoomType,
        int Capacity,
        decimal PricePerNight,
        string Description,
        string Amenities) 
        : IRequest<Result<Guid>>;
}
