using Application.Common.Interfaces.Repositories;
using Application.DiscountFactory.Interfaces;
using Application.DiscountStrategy;
using Domain.Strategies;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DiscountFactory
{
    public class LoyaltyDiscountStrategyFactory(
        ILoyaltyRepository loyaltyRepository) : IDiscountStrategyFactory
    {
        public DiscountType Type => DiscountType.LoyaltyProgram;

        public async Task<IDiscountStrategy> CreateAsync(BookingDiscountEnvironment discountEnvironment)
        {
            var loyalty = await loyaltyRepository.GetByUserIdAsync(discountEnvironment.UserId, default);
            if (loyalty == null)
                return new EmptyDiscountStrategy();

            return new TierDiscountStrategy(loyalty.GetTierDiscountPercent());
        }
    }
}
