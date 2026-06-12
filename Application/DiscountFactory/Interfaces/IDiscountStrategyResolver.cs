using Domain.Strategies;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DiscountFactory.Interfaces
{
    public interface IDiscountStrategyResolver
    {
        Task<IDiscountStrategy> ResolveAsync(BookingDiscountEnvironment discountEnvironment, DiscountType discountType);
    }
}
