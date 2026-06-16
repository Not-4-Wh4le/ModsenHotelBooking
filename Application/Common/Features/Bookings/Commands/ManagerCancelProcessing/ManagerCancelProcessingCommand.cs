using Application.Common.Security;
using Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Features.Bookings.Commands.ManagerCancelProcessing
{
    [Authorize(UserRole.HotelManager)]
    public record ManagerCancelProcessingCommand(
        Guid BookingId,
        bool IsConfirm) : IRequest<Result<Guid>>; 
}
