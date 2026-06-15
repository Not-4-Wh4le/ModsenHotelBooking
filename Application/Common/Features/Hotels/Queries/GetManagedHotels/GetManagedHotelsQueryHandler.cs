using Application.Common.Interfaces;
using Application.Common.Interfaces.Repositories;
using Application.Common.Models;
using Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Features.Hotels.Queries.GetManagedHotels
{
    public class GetManagedHotelsQueryHandler(
        IHotelRepository hotelRepository, 
        IUserRepository userRepository,
        ICurrentUserService currentUser)
        : IRequestHandler<GetManagedHotelsQuery, Result<PagedResultDto<ManagedHotelsDto>>>
    {
        public async Task<Result<PagedResultDto<ManagedHotelsDto>>> Handle(GetManagedHotelsQuery request, CancellationToken cancellationToken)
        {
            var targetManagerId = request.ManagerId;
            if (currentUser.Role == nameof(UserRole.HotelManager))
            {
                if (request.ManagerId.HasValue && request.ManagerId.Value != currentUser.Id)
                    return Result<PagedResultDto<ManagedHotelsDto>>.Failure("Forbidden: You do not have access");

                targetManagerId = currentUser.Id;
            }
            
            var (hotels, totalCount) = await hotelRepository.GetManagedAsync(
                targetManagerId,     
                request.MinRating, 
                request.Page, 
                request.PageSize,
                request.SortBy, 
                request.IsDescending, 
                cancellationToken);
            
            var managerIds = hotels.Select(h => h.ManagerId).Distinct().ToList();
            var managers = await userRepository.GetByIdsAsync(managerIds, cancellationToken);
            var managersDict = managers.ToDictionary(m => m.Id, m => m);

            var dtos = hotels.Select(hotel => new ManagedHotelsDto(
                hotel.Id,
                hotel.Name,
                hotel.City,
                hotel.Country,
                hotel.Address,
                hotel.Rating,
                hotel.ManagerId,
                managersDict.TryGetValue(hotel.ManagerId, out var manager) ? manager.Username : "Unknown"))
                .ToList();

            var result = new PagedResultDto<ManagedHotelsDto>(dtos, totalCount, request.Page, request.PageSize);
            return Result<PagedResultDto<ManagedHotelsDto>>.Success(result);
        }   
    }
}

