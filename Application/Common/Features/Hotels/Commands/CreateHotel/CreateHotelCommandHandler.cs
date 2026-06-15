using Application.Common.Interfaces;
using Application.Common.Interfaces.Repositories;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Features.Hotels.Commands.CreateHotel
{
    public class CreateHotelCommandHandler(
        IHotelRepository hotelRepository,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork)
        : IRequestHandler<CreateHotelCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(CreateHotelCommand request, CancellationToken cancellationToken)
        {
            Guid hotelManagerId;

            if (currentUser.Role == UserRole.Admin.ToString())
            {
                if (request.ManagerId == Guid.Empty || request.ManagerId == null)
                    return Result<Guid>.Failure("Admin must specify valid ManagerId");

                hotelManagerId = request.ManagerId.Value;
            }

            else
                hotelManagerId = currentUser.Id!.Value;
            Hotel hotel;
            try
            {
                hotel = new Hotel(
                   Guid.NewGuid(),
                   request.Name,
                   request.City,
                   request.Country,
                   request.Address,
                   hotelManagerId);
            }
            catch (ArgumentException ex)
            {
                return Result<Guid>.Failure(ex.Message);
            }
            await hotelRepository.AddAsync(hotel, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<Guid>.Success(hotel.Id);
        }
    }
}
