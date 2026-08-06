using Application.Common.Interfaces;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleUI.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        public Guid? Id { get; private set; }
        public string? Username { get; private set; }
        public string? Role { get; private set; }
        public string? Email { get; private set; }
        public bool IsAuthenticated => Id.HasValue;

        public void Authenticate(Guid userId, string username, string email, string role)
        {
            Id = userId;
            Username = username;
            Role = role;
            Email = email;
        }
       

        public void Logout()
        {
            Id = null;
            Username = null;
            Role = null;
            Email = null;
        }
    }
}
