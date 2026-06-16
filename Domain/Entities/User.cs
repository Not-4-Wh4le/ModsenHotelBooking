using Domain.Common;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class User : IEntity
    {
        public Guid Id { get; init; }
        public string Username { get; private set;} = string.Empty;
        public string Email { get; private set; }
        public string PasswordHash { get; private set; } = string.Empty;
        public UserRole Role { get; private set;} = UserRole.Customer;
        public bool IsDeleted { get; private set;} = false;
        public DateTimeOffset CreatedAt { get; } = DateTimeOffset.Now;

        public User(Guid id,  string username, string email, string passwordHash, UserRole role = UserRole.Customer)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Id cannot be empty");
   
            if (String.IsNullOrEmpty(email))
                throw new ArgumentException("Email cannot be empty");

            if (String.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("Invalid password hash");
            
            Id = id;
            Email = email;
            ChangeUsername(username);
            ChangePassword(passwordHash);
            ChangeRole(role);
        }

        public void ChangeUsername(string username)
        {
            ValidateUsername(username);

            Username = username;
        }

        public void ChangePassword(string passwordHash)
        {
            if (String.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("Invalid password hash");
            PasswordHash = passwordHash;
        }

        public void DeleteUser()
        {
            if (IsDeleted)
                throw new InvalidOperationException("User has already been deleted");
            IsDeleted = true;
        }

        public void ChangeRole(UserRole role) => Role = role;

        private void ValidateUsername(string username)
        {
            if (String.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be empty");

            if (username.Length < 6)
                throw new ArgumentException("Usetname cannot be shorter then 6 chatacters");

            if (username.Contains(' '))
                throw new ArgumentException("Username cannot contain spaces.");
        }
    }
}
