using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Features.Users.Commands.LoginUser
{
    public class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
    {
        public LoginUserCommandValidator()
        {
            RuleFor(c => c.Username)
                .NotEmpty().WithMessage("Username is required");

            RuleFor(c => c.Password)
                .NotEmpty().WithMessage("Password is required");
                
        }
    }
}
