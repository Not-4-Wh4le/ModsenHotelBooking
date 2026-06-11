using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Interfaces.Repositories
{
    public interface ILoyaltyRepository : IRepositoryBase<LoyaltyProgram>
    {
        Task<LoyaltyProgram?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    }
}
