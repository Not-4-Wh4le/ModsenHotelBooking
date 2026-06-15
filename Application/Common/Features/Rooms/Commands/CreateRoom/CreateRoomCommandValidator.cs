using Application.Common.Interfaces.Repositories;
using Domain.Enums;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Features.Rooms.Commands.CreateRoom
{
    public class CreateRoomCommandValidator : AbstractValidator<CreateRoomCommand>
    {
        public CreateRoomCommandValidator()
        {
            RuleFor(c => c.HotelId)
                .NotEmpty().WithMessage("Hotel Id is required");

            RuleFor(c => c.Number)
            .GreaterThan(0).WithMessage("Room number must be positive");

            RuleFor(c => c.RoomType)
                .NotEmpty().WithMessage("Room type is required")
                .IsEnumName(typeof(RoomType), caseSensitive: false)
                .WithMessage($"Invalid room type");

            RuleFor(c => c.Capacity)
                .GreaterThan(0).WithMessage("Capacity must be positive");

            RuleFor(c => c.PricePerNight)
                .GreaterThan(0).WithMessage("Price per night must be positive");

            RuleFor(c => c.Area)
                .GreaterThan(0).WithMessage("Area must be positive");

            RuleFor(c => c.Description)
              .NotNull().WithMessage("Description property cannot be null")
              .MaximumLength(500).WithMessage("Description string cannot exceed 500 characters");

            RuleFor(c => c.Amenities)
                .NotNull().WithMessage("Amenities property cannot be null")
                .MaximumLength(250).WithMessage("Amenities string cannot exceed 250 characters");

        }
    }
}
