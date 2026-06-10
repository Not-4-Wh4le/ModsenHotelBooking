using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Strategies
{
    public class PromoCodeDiscountStrategy(decimal fixedDiscountPercent): IDiscountStrategy
    {
        public decimal CalculateDiscount(decimal bookingTotalPrice, int currentPoints, out int pointsToDeduct)
        {
            pointsToDeduct = 0;
            return bookingTotalPrice * fixedDiscountPercent;
        }
    }
}
