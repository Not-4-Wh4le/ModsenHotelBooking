using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Strategies
{
    public class TierDiscountStrategy(decimal tierDiscountPercent) : IDiscountStrategy
    {
        public decimal CalculateDiscount(decimal bookingTotalPrice, int currentPoints, out int pointsToDeduct)
        {
            pointsToDeduct = 0;
            if (currentPoints <= 0)
                return 0m;

            var maxAvailableDiscount = bookingTotalPrice * tierDiscountPercent;
            pointsToDeduct = Math.Min((int)maxAvailableDiscount, currentPoints);
            return pointsToDeduct;
        }
    }
}
