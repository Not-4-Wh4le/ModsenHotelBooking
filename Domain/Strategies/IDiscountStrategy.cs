using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Strategies
{
    public interface IDiscountStrategy
    {
        decimal CalculateDiscount(decimal bookingTotalPrice, int currentPoints, out int pointsToDeduct);
    }
}
