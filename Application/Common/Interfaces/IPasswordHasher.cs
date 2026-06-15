using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Interfaces
{
    public interface IPasswordHasher
    {
        string HashPassword(string password);
        public bool VerifyPassword(string password, string passwordHash);
    }
}
