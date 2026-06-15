using Application.Common.Models;
using Application.Common.Security;
using Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Features.Hotels.Queries.GetManagedHotels
{
    [Authorize(UserRole.HotelManager, UserRole.Admin)]
    public record GetManagedHotelsQuery(
        Guid? ManagerId = null,
        double? MinRating = null,
        string SortBy = "Name",
        bool IsDescending = false,
        int Page = 1, 
        int PageSize = 10)
        : IRequest<Result<PagedResultDto<ManagedHotelsDto>>>;
}
