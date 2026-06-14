using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Features.Users.Commands.DeleteUser
{
    public record DeleteUserCommand(
       string Username) : IRequest<Result<Guid>>;
}
