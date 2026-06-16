using Application.Common.Interfaces.Repositories;
using Domain.Common.Events;
using Domain.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Features.LoyaltyPrograms.BookingCanceled
{
    public class BookingCanceledEventHandler(
        ILoyaltyRepository loyaltyRepository) : IDomainEventHandler<BookingCancelledEvent>
    {
        public async Task Handle(BookingCancelledEvent notification, CancellationToken cancellationToken)
        {
            if (notification.PointsToRefund == 0)
                return;
            var loyalty = await loyaltyRepository.GetByUserIdAsync(notification.UserId, cancellationToken);
            if(loyalty != null)
            {
                loyalty.RefundPoints(notification.PointsToRefund);
                loyaltyRepository.Update(loyalty);
            }
        }
    }
}
