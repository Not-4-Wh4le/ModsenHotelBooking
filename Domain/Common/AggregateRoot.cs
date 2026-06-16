using Domain.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Common
{
    public class AggregateRoot
    {
        private readonly List<IDomainEvent> domainEvents = [];
        public IReadOnlyCollection<IDomainEvent> DomainEvents => domainEvents.AsReadOnly();
        protected void AddDomainEvent(IDomainEvent domainEvent) =>
            domainEvents.Add(domainEvent);

        public void ClearDomainEvents() => domainEvents.Clear();
    }
}
