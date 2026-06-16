using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
namespace Domain.Common.Interfaces
{
    public interface IDomainEventHandler<in T> : INotificationHandler<T> where T : IDomainEvent;
}
