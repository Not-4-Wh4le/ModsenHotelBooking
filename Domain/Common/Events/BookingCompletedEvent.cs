using Domain.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Common.Events
{
    public record BookingCompletedEvent(
        DateTimeOffset Timestamp,
        Guid UserId,
        decimal FinalPrice)
        : IDomainEvent;

}
