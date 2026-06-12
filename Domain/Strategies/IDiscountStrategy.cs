using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Strategies
{
    public interface IDiscountStrategy
    {
        DiscountResult CalculateDiscount(DiscountContext discountContext);
    }
}
