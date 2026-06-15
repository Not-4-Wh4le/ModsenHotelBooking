using Application.Common.Interfaces.Repositories;
using Application.Common.Models;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Features.Hotels.Queries.SearchHotels
{
    public class SearchHotelsQueryHandler(
        IHotelRepository hotelRepository,
        IMapper mapper)
        : IRequestHandler<SearchHotelsQuery, Result<PagedResultDto<HotelDto>>>
    {
        public async Task<Result<PagedResultDto<HotelDto>>> Handle(SearchHotelsQuery request, CancellationToken cancellationToken)
        {
            var (hotels, totalCount) = await hotelRepository.SearchAsync(
                request.City, 
                request.Country,
                request.MinRating, 
                request.Page, 
                request.PageSize,
                request.SortBy, 
                request.IsDescending, 
                cancellationToken);

            var dtos = mapper.Map<IReadOnlyList<HotelDto>>(hotels);
            var result = new PagedResultDto<HotelDto>(dtos, totalCount, request.Page, request.PageSize);
            return Result<PagedResultDto<HotelDto>>.Success(result);

        }
    }
}
