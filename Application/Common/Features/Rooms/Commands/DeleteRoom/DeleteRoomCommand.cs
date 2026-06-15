using Application.Common.Security;
using Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Features.Rooms.Commands.DeleteRoom
{
    [Authorize(UserRole.Admin, UserRole.HotelManager)]
    public record DeleteRoomCommand(Guid Id)
        : IRequest<Result<Guid>>;
}
