using Application.Common.Security;
using Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Features.Users.Commands.ChangeUserRole
{
    [Authorize(UserRole.Admin)]
    public record ChangeUserRoleCommand(
        string Username,
        string NewRole) : IRequest<Result<Guid>>;
}
