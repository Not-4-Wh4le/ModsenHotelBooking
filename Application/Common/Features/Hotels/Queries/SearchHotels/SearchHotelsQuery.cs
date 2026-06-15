using Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Features.Hotels.Queries.SearchHotels
{
    public record SearchHotelsQuery(
        string? City = null,
        string? Country = null,
        double? MinRating = null,
        string SortBy = "Name",
        bool IsDescending = false,
        int Page = 1,
        int PageSize = 10)
        : IRequest<Result<PagedResultDto<HotelDto>>>;
}
