using Application.Common.Interfaces.Repositories;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Features.Hotels.Commands.CreateHotel
{
    public class CreateHotelCommandValidator : AbstractValidator<CreateHotelCommand>
    {
        public CreateHotelCommandValidator(IHotelRepository hotelRepository)
        {
            RuleFor(c => c.Name)
            .NotEmpty()
            .MinimumLength(6).WithMessage("Hotel name must be at least 6 characters long");

            RuleFor(c => c.City).NotEmpty();
            RuleFor(c => c.Country).NotEmpty();
            RuleFor(c => c.Address).NotEmpty();

            RuleFor(c => c)
                .MustAsync(async (command, cancelationToken) =>
                await hotelRepository.IsNameUniqueInCityAsync(command.Name, command.City, cancelationToken))
                .WithMessage("Hotel with this name already exists in this city");
        }
    }
}
