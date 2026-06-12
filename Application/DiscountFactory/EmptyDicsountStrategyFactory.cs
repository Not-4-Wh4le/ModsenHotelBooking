using Application.DiscountFactory.Interfaces;
using Application.DiscountStrategy;
using Domain.Strategies;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DiscountFactory
{
    public class EmptyDicsountStrategyFactory : IDiscountStrategyFactory
    {
        public DiscountType Type => DiscountType.NoDiscount;

        public Task<IDiscountStrategy> CreateAsync(BookingDiscountEnvironment discountEnvironment)
        {
            return Task.FromResult<IDiscountStrategy>(new EmptyDiscountStrategy());
        }
    }
}
