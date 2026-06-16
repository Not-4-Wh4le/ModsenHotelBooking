using Application.Common.Interfaces;
using Application.Common.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Features.Bookings.Commands.CustomerCancelRequest
{
    public class CustomerCancelRequestCommandHandler(
        ICurrentUserService currentUser,
        IBookingRepository bookingRepository)
        : IRequestHandler<CustomerCancelRequestCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(CustomerCancelRequestCommand request, CancellationToken cancellationToken)
        {
            var booking = await bookingRepository.GetByIdAsync(request.BookingId, cancellationToken);
            if(booking == null)
                return Result<Guid>.Failure("Booking not found");

            if(booking.UserId != currentUser.Id)
                return Result<Guid>.Failure("Forbidden: You can not cancel not your booking");

            try
            {
                booking.RequestCancelation();
            }
            catch (InvalidOperationException ex)
            {
                return Result<Guid>.Failure(ex.Message);
            }
            bookingRepository.Update(booking);

            return Result<Guid>.Success(booking.Id);
        }
    }
}
