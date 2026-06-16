using Application.Common.Interfaces;
using Application.Common.Interfaces.Repositories;
using Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Features.Bookings.Commands.ConfirmBooking
{
    public class ConfirmBookingCommandHandler(
        IBookingRepository bookingRepository,
        IDomainEventDispathcer eventDispatcher)
        : IRequestHandler<ConfirmBookingCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(ConfirmBookingCommand request, CancellationToken cancellationToken)
        {
            var booking = await bookingRepository.GetByIdAsync(request.BookingId, cancellationToken);
            if (booking == null)
                return Result<Guid>.Failure("Booking not found");

            try
            {
                booking.ConfirmBooking();
            }
            catch (InvalidOperationException ex)
            {
                return Result<Guid>.Failure(ex.Message);
            }

            bookingRepository.Update(booking);
            await eventDispatcher.DispatchAndClearAsync(booking, cancellationToken);
            return Result<Guid>.Success(booking.Id);
        }
    }
}
