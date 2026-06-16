using Application.Common.Interfaces;
using Application.Common.Interfaces.Repositories;
using Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Features.Bookings.Commands.ManagerCancelProcessing
{
    public class ManagerCancelProcessingCommandHandler(
        IBookingRepository bookingRepository,
        IDomainEventDispathcer eventDispathcer
        )
        : IRequestHandler<ManagerCancelProcessingCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(ManagerCancelProcessingCommand request, CancellationToken cancellationToken)
        {
            var booking = await bookingRepository.GetByIdAsync(request.BookingId, cancellationToken);
            if (booking == null) 
                return Result<Guid>.Failure("Booking not found");

            try
            {
                if (request.IsConfirm)
                    booking.ConfirmCancelation();
                else
                    booking.RejectCancelation();
                
            }
            catch (InvalidOperationException ex)
            {
                return Result<Guid>.Failure(ex.Message);
            }
            bookingRepository.Update(booking);
            await eventDispathcer.DispatchAndClearAsync(booking, cancellationToken);
            return Result<Guid>.Success(booking.Id);
        }
    }
}
