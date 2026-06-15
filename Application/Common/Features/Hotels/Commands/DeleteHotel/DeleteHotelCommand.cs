using Application.Common.Security;
using Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Features.Hotels.Commands.DeleteHotel
{
    [Authorize(UserRole.Admin, UserRole.HotelManager)]
    public record DeleteHotelCommand(
        Guid Id)
        : IRequest<Result<Guid>>;
}
