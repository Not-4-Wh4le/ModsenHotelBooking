using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Features.Users.Commands.RegisterUser
{
    public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserCommandValidator()
        {
            RuleFor(c => c.Username)
                .NotEmpty().WithMessage("Username cannot be empty")
                .MinimumLength(6).WithMessage("Username cannot be shorter then 6 characters")
                .MaximumLength(30).WithMessage("Username cannot be longer then 30 characters");

            RuleFor(c => c.Password)
                .NotEmpty().WithMessage("Password cannot be empty")
                .MinimumLength(6).WithMessage("Password cannot be shorter then 6 characters")
                .Matches(@"[A-Z]").WithMessage("Password must contain at least one capital letter")
                .Matches(@"[0-9]").WithMessage("Password must contain at least number");

            RuleFor(c => c.Email)
                .NotEmpty().WithMessage("Email cannot be empty")
                .EmailAddress().WithMessage("Incorrect Email address format");

                
        }
    }
}
