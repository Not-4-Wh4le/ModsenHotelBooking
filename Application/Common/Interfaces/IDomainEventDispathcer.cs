using Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Interfaces
{
    public interface IDomainEventDispathcer
    {
        Task DispatchAndClearAsync(AggregateRoot entity, CancellationToken cancellationToken);
    }
}
