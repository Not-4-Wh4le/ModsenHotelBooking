using Domain.Strategies;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DiscountFactory.Interfaces
{
    public interface IDiscountStrategyFactory
    {
        DiscountType Type { get; }
        Task<IDiscountStrategy> CreateAsync(BookingDiscountEnvironment discountEnvironment);
    }
}
