using Application.Common.Interfaces.Repositories;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Features.Hotels.Commands.UpdateHotel
{
    public class UpdateHotelCommandValidator : AbstractValidator<UpdateHotelCommand>
    {
        public UpdateHotelCommandValidator()
        {
            RuleFor(c => c.Id).NotEmpty().WithMessage("Id is required");
            
            RuleFor(c => c.NewName)
            .MinimumLength(6).WithMessage("Hotel name must be at least 6 characters long");

            RuleFor(c => c.NewManagerId)
            .NotEqual(Guid.Empty)
            .When(c => c.NewManagerId.HasValue)
            .WithMessage("Manager ID cannot be an empty GUID");
        }
    }
}
