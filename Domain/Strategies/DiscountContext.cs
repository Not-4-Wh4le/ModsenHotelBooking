using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Strategies
{
    public record DiscountContext(decimal TotalPrice, int CurrentPoints, string? PromoCode);
}
