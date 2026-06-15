using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Features.Users.Queries.GetUsersList
{
    public record UserDto(
        Guid Id,
        string Username,
        string Email,
        string Role,
        bool IsDeleted);
}
