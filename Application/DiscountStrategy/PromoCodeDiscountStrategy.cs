using Domain.Strategies;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DiscountStrategy
{
    public class PromoCodeDiscountStrategy(decimal fixedDiscountPercent): IDiscountStrategy
    {
        public DiscountResult CalculateDiscount(DiscountContext discountContext)
        {
            var discount = discountContext.TotalPrice * fixedDiscountPercent;
          
            return new DiscountResult(discount, 0);
        }
    }
}
