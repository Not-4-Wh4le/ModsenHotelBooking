using Application.Common.Interfaces.Repositories;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Interfaces
{
    public interface IUserRepository : IRepositoryBase<User>
    {
        Task<(IReadOnlyList<User> Items, int TotalCount)> GetPagedAsync(
            int page,
            int pageSize,
            CancellationToken cancellationToken);
        Task<User> GetUserByEmailAsync(string Email, CancellationToken cancellationToken);
        Task<User> GetUserByUsernameAsync(string Username, CancellationToken cancellationToken);

    }
}
