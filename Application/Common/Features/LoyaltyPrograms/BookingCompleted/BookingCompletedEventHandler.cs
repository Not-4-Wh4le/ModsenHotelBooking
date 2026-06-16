using Application.Common.Interfaces.Repositories;
using Domain.Common.Events;
using Domain.Common.Interfaces;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Features.LoyaltyPrograms.BookingCompleted
{
    public class BookingCompletedEventHandler(
        ILoyaltyRepository loyaltyRepository): IDomainEventHandler<BookingCompletedEvent>
    {
        public async Task Handle(BookingCompletedEvent notification, CancellationToken cancellationToken)
        {
            var loyalty = await loyaltyRepository.GetByUserIdAsync(notification.UserId, cancellationToken)
                ?? new LoyaltyProgram(Guid.NewGuid(), notification.UserId);

            loyalty.RewardPointsForBooking(notification.FinalPrice);
            await loyaltyRepository.CreateOrUpdateAsync(loyalty, cancellationToken);
        }
    }
}

