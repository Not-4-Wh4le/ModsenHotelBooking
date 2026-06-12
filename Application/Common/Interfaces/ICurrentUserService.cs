using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Interfaces
{
    public interface ICurrentUserService
    {
        Guid? Id { get; }
        string? Username { get; }
        string? Role {  get; }
        string? Email { get; }
    }
}
