using Application.Common.Security;
using Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Features.Users.Commands.DeleteUser
{
    [Authorize(UserRole.Admin)]
    public record DeleteUserCommand(
       string Username) : IRequest<Result<Guid>>;
}
