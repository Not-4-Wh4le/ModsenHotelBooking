using Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Features.Bookings.Queries.GetCustomerBookingHistory
{
    public record GetCustomerBookingHistoryQuery(
        Guid CustomerId,
        int Page = 1,
        int PageSize = 10)
        : IRequest<Result<PagedResultDto<CustomerBookingHistoryDto>>>;
}
