using Application.Common.Interfaces;
using Application.Common.Interfaces.Repositories;
using Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Features.Hotels.Commands.DeleteHotel
{
    public class DeleteHotelCommandHandler(
        IHotelRepository hotelRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser)
        : IRequestHandler<DeleteHotelCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(DeleteHotelCommand request, CancellationToken cancellationToken)
        {
            var hotel = await hotelRepository.GetByIdAsync(request.Id, cancellationToken);

            if (hotel == null)
                return Result<Guid>.Failure("Hotel not found");

            if (currentUser.Role == nameof(UserRole.HotelManager) && currentUser.Id != hotel.ManagerId)
                return Result<Guid>.Failure("Forbidden: You are not the manager of this hotel");

            if(hotel.IsDeleted)
                return Result<Guid>.Failure("Hotel is already deleted");
            
            hotel.DeleteHotel();
            hotelRepository.Update(hotel);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<Guid>.Success(hotel.Id);
        }
    }
}
