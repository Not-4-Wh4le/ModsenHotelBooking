using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Features.Bookings.Commands.ManagerCancelProcessing
{
    public class ManagerCancelProcessingCommandValidator : AbstractValidator<ManagerCancelProcessingCommand>
    {
        public ManagerCancelProcessingCommandValidator()
        {
            RuleFor(c => c.BookingId)
               .NotEmpty()
               .WithMessage("Booking Id cannot be empty");

            RuleFor(c => c.IsConfirm)
               .NotEmpty()
               .WithMessage("Confirm is required");
        }
    }
}
