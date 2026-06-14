using Application.Common.Interfaces;
using Application.Common.Interfaces.Repositories;
using Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Features.Hotels.Commands.UpdateHotel
{
    public class UpdateHotelCommandHandler(
        IHotelRepository hotelRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser)
        : IRequestHandler<UpdateHotelCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(UpdateHotelCommand request, CancellationToken cancellationToken)
        {
            var hotel = await hotelRepository.GetById(request.Id, cancellationToken);
            if (hotel == null)
                return Result<Guid>.Failure("Hotel not found");
            
            if(hotel.IsDeleted)
                return Result<Guid>.Failure("Cannot update deleted hotel");

            if(currentUser.Role == nameof(UserRole.HotelManager) && hotel.ManagerId != currentUser.Id)
                return Result<Guid>.Failure("Forbidden: You are not the manager of this hotel");

            try
            {
                if(hotel.Name != request.NewName)
                {
                    bool isUnique = await hotelRepository.IsNameUniqueInCityAsync(
                        request.NewName, hotel.City, cancellationToken);
                    if(!isUnique)
                        return Result<Guid>.Failure("Hotel with this name already exists in this city");
                    hotel.ChangeName(request.NewName);
                }
                
                if (currentUser.Role == nameof(UserRole.Admin) && request.NewManagerId.HasValue)
                {
                    var newManagerId = request.NewManagerId.Value;
                    var newManager = await userRepository.GetById(newManagerId, cancellationToken);

                    if(newManager == null)
                        return Result<Guid>.Failure("Manager not found");

                    if (newManager.Role != UserRole.HotelManager)
                        return Result<Guid>.Failure("The specified user does not have the HotelManager roles");

                    hotel.ChangeManager(newManagerId);
                }
            }
            catch (ArgumentException ex)
            {
                return Result<Guid>.Failure(ex.Message);
            }

            hotelRepository.Update(hotel);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<Guid>.Success(hotel.Id);
        }
    }
}
