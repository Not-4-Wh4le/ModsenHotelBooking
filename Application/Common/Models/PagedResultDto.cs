using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Models
{
    public record PagedResultDto<T>(
        IReadOnlyList<T> Items,
        int TotalCount,
        int Page,
        int PageSize)
    {
        public int TotalPages => (int)Math.Ceiling((double)(TotalCount) / PageSize);
        public bool HasNextPage => TotalPages > Page;
        public bool HasPreviousPage => Page > 1;
    }
}
