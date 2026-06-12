using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Strategies
{
    public record DiscountResult(
        decimal DiscountAmount, int PointsToDeduct);
}
