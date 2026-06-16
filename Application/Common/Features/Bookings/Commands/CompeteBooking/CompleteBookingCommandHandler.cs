using Application.Common.Interfaces;
using Application.Common.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Features.Bookings.Commands.CompeteBooking
{
    public class CompleteBookingCommandHandler(
        IBookingRepository bookingRepository,
        IDomainEventDispathcer eventDispathcer)
        : IRequestHandler<CompleteBookingCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(CompleteBookingCommand request, CancellationToken cancellationToken)
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
            await eventDispathcer.DispatchAndClearAsync(booking, cancellationToken);
            return Result<Guid>.Success(booking.Id);

        }
    }
}
