using Application.DiscountFactory.Interfaces;
using Domain.Strategies;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DiscountFactory
{
    public class PromoCodeDiscountStrategyFactory : IDiscountStrategyFactory
    {
        public DiscountType Type => DiscountType.PromoCode;

        public Task<IDiscountStrategy> CreateAsync(BookingDiscountEnvironment discountEnvironment)
        {
            throw new NotImplementedException();
        }
    }
}
