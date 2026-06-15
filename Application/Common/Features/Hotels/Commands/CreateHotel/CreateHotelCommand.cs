using Application.Common.Security;
using Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Features.Hotels.Commands.CreateHotel
{
    [Authorize(UserRole.Admin, UserRole.HotelManager)]
    public record CreateHotelCommand(
        string Name,
        string City,
        string Country,
        string Address,
        Guid? ManagerId)
        : IRequest<Result<Guid>>;
}
