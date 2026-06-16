using Domain.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Common.Events
{
    public record BookingCancelledEvent(
        DateTimeOffset Timestamp,
        Guid BookingId,
        Guid UserId,
        int PointsToRefund) 
        : IDomainEvent;
}
