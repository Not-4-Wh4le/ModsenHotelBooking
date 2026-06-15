using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Features.Users.Commands.LoginUser
{
    public record UserSessionDto(
        Guid Id,
        string Username,
        string Email,
        string Role);
}
