using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Security
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class AuthorizeAttribute : Attribute
    {
        public string Roles { get; set; } = string.Empty;

        public AuthorizeAttribute(params UserRole[] roles)
        {
            if (roles != null && roles.Length > 0)
                Roles = string.Join(", ", roles.Select(r => r.ToString()));
        }
    }
}
