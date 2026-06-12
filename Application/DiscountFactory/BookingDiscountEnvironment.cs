using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DiscountFactory
{
    public record BookingDiscountEnvironment(
        Guid UserId,
        Guid RoomId,
        DateTimeOffset CheckInDate,
        string? PromoCode
        );
}
