using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Features.Bookings.Queries.GetHotelsBooking
{
    public record ManagerBookingDto(
     Guid Id,
     string GuestUsername,
     string GuestEmail,
     int RoomNumber,
     DateTimeOffset CheckInDate,
     DateTimeOffset CheckOutDate,
     decimal FinalPrice,
     string Status
     );
}
