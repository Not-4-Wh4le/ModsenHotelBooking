using Application.Common.Security;
using Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Features.Bookings.Commands.CompeteBooking
{
    [Authorize(UserRole.HotelManager)]
    public record CompleteBookingCommand(
        Guid BookingId) : IRequest<Result<Guid>>;
}
