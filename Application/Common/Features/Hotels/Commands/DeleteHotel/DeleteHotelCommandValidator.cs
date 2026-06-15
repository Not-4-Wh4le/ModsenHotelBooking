using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Features.Hotels.Commands.DeleteHotel
{
    public class DeleteHotelCommandValidator : AbstractValidator<DeleteHotelCommand>
    {
        public DeleteHotelCommandValidator()
        {
            RuleFor(c => c.Id)
                .NotEmpty().WithMessage("Id is required");
        }
    }
}
