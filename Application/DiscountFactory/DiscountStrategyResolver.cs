using Application.DiscountFactory.Interfaces;
using Domain.Strategies;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DiscountFactory
{
    public class DiscountStrategyResolver(
        IEnumerable<IDiscountStrategyFactory> factories): IDiscountStrategyResolver
    {
        public async Task<IDiscountStrategy> ResolveAsync(BookingDiscountEnvironment discountEnvironment, DiscountType discountType)
        {
            var factory  = factories.FirstOrDefault(f => f.Type == discountType)
                ?? factories.First(f => f.Type == DiscountType.NoDiscount);

            return await factory.CreateAsync(discountEnvironment);
        }
    }
}
