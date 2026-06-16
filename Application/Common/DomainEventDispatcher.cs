using Application.Common.Interfaces;
using Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common
{
    public class DomainEventDispatcher(IMediator mediator) : IDomainEventDispathcer
    {
        public async Task DispatchAndClearAsync(AggregateRoot entity, CancellationToken cancellationToken)
        {
            var events = entity.DomainEvents.ToList();
            foreach(var domainEvent in events)
                await mediator.Publish(domainEvent, cancellationToken);
            
            entity.ClearDomainEvents();
        }
    }
}
