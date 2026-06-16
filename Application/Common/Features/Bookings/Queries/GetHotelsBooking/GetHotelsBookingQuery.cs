using Application.Common.Models;
using Application.Common.Security;
using Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Features.Bookings.Queries.GetHotelsBooking
{
    [Authorize(UserRole.HotelManager)]
    public record GetHotelsBookingQuery(
       Guid HotelId,
       int Page = 1,
       int PageSize = 10)
       : IRequest<Result<PagedResultDto<ManagerBookingDto>>>;
}
