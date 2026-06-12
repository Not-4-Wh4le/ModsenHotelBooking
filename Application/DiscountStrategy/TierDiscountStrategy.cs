using Domain.Entities;
using Domain.Strategies;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DiscountStrategy
{
    public class TierDiscountStrategy(decimal tierDiscountPercent) : IDiscountStrategy
    {
        public DiscountResult CalculateDiscount(DiscountContext discountContext)
        {
            if (discountContext.CurrentPoints <= 0)
                return new DiscountResult(0m, 0); 

            var maxAvailableDiscount = discountContext.TotalPrice * tierDiscountPercent;
            var pointsToDeduct = Math.Min((int)maxAvailableDiscount, discountContext.CurrentPoints);
            return new DiscountResult(pointsToDeduct, pointsToDeduct);
        }
    }
}
