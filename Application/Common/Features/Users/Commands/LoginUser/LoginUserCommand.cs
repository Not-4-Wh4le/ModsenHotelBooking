using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Features.Users.Commands.LoginUser
{
    public record LoginUserCommand(
        string Username,
        string Password) : IRequest<Result<UserSessionDto>>;
}
