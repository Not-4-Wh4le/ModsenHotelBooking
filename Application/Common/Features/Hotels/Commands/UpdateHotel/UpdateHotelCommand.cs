using Application.Common.Security;
using Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Features.Hotels.Commands.UpdateHotel
{
    [Authorize(UserRole.Admin, UserRole.HotelManager)]
    public record UpdateHotelCommand(
        Guid Id,
        string NewName,
        Guid? NewManagerId
        ) : IRequest<Result<Guid>>;
}
