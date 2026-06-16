using Application.Common.Security;
using Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Features.Bookings.Commands.ConfirmBooking
{
    [Authorize(UserRole.HotelManager)]
    public record ConfirmBookingCommand(
        Guid BookingId) 
        : IRequest<Result<Guid>>;
}
