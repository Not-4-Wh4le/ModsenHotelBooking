using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Features.Bookings.Commands.CustomerCancelRequest
{
    public class CustomerCancelRequestCommandValidator : AbstractValidator<CustomerCancelRequestCommand>
    {
        public CustomerCancelRequestCommandValidator()
        {
            RuleFor(c => c.BookingId)
                .NotEmpty()
                .WithMessage("Booking Id cannot be empty");
        }
    }
}
