using Domain.Strategies;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DiscountStrategy
{
    public class EmptyDiscountStrategy : IDiscountStrategy
    {
        public DiscountResult CalculateDiscount(DiscountContext discountContext)
        {
            return new DiscountResult(0m ,0);
        }
    }
}
