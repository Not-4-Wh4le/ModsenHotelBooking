using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Features.Bookings.Queries.GetCustomerBookingHistory
{
    public record CustomerBookingHistoryDto(
         Guid Id,
         string HotelName,
         string RoomType,
         int RoomNumber,
         DateTimeOffset CheckInDate,
         DateTimeOffset CheckOutDate,
         decimal FinalPrice,
         string Status
         );
}
