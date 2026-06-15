using Domain.Enums;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Features.Rooms.Commands.UpdateRoom
{
    public class UpdateRoomCommandValidator : AbstractValidator<UpdateRoomCommand>
    {
        public UpdateRoomCommandValidator()
        {
            RuleFor(c => c.Id)
            .NotEmpty().WithMessage("Room Id is required");

            RuleFor(c => c.Number)
                .GreaterThan(0).WithMessage("Room number must be positive");

            RuleFor(c => c.RoomType)
                .NotEmpty().WithMessage("Room type is required")
                .IsEnumName(typeof(RoomType), caseSensitive: false)
                .WithMessage("Invalid room type");

            RuleFor(c => c.Capacity)
                .GreaterThan(0).WithMessage("Capacity must be positive");

            RuleFor(c => c.PricePerNight)
                .GreaterThan(0).WithMessage("Price per night must be positive");

            RuleFor(c => c.Description)
                .NotNull().WithMessage("Description cannot be null")
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");

            RuleFor(c => c.Amenities)
                .NotNull().WithMessage("Amenities cannot be null")
                .MaximumLength(250).WithMessage("Amenities cannot exceed 250 characters");
        }
    }
}
