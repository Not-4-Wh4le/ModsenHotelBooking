using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Features.Bookings.Commands.CreateBooking
{
    public record CreateBookingCommand(
        Guid UserId,
        Guid RoomId,
        DateTimeOffset CheckInDate,
        DateTimeOffset CheckOutDate,
        string? PromoCode,
        bool UseLoyaltyPoints)
        : IRequest<Result<Guid>>;
}
