using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Features.Bookings.Commands.CreateBooking
{
    public class CreateBookingCommandValidator : AbstractValidator<CreateBookingCommand>
    {
        public CreateBookingCommandValidator()
        {
            RuleFor(c => c.UserId)
            .NotEmpty().WithMessage("User Id is required");

            RuleFor(c => c.RoomId)
                .NotEmpty().WithMessage("Room Id is required");

            RuleFor(c => c.CheckInDate)
                .GreaterThanOrEqualTo(DateTimeOffset.UtcNow)
                .WithMessage("Check-in date cannot be in the past");

            RuleFor(c => c.CheckOutDate)
                .GreaterThan(c => c.CheckInDate)
                .WithMessage("Check-out date must be strictly after the check-in date");

            RuleFor(c => c)
                .Must(c => (c.CheckOutDate - c.CheckInDate).Days >= 1)
                .WithMessage("Booking must be for at least one night")
                .WithName("Dates");

            RuleFor(c => c)
                .Must(c => string.IsNullOrEmpty(c.PromoCode) || !c.UseLoyaltyPoints)
                .WithMessage("You cannot use a promo code and loyalty points at the same time")
                .WithName("Discounts");

            RuleFor(c => c.PromoCode)
                .Must(promo => promo == null || !string.IsNullOrWhiteSpace(promo))
                .WithMessage("Promo code cannot be empty spaces.");
        }
    }
}
