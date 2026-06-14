using Application.Common.Interfaces;
using Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Features.Users.Commands.ChangeUserRole
{
    public class ChangeUserRoleCommandHandler(
        IUserRepository userRepository,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork)
        : IRequestHandler<ChangeUserRoleCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(ChangeUserRoleCommand request, CancellationToken cancellationToken)
        {
            if (!Enum.TryParse<UserRole>(request.NewRole, ignoreCase: true, out var parsedNewRole))
                return Result<Guid>.Failure($"Role {request.NewRole} does not exist");

            var user = await userRepository.GetUserByUsernameAsync(request.Username, cancellationToken);
            
            if (user == null)
                return Result<Guid>.Failure("User with this username does not exist");

            if(user.IsDeleted)
                return Result<Guid>.Failure("Cannot change role for deleted user");

            if(user.Id == currentUser.Id)
                return Result<Guid>.Failure("Admin cannot demote themselves");

            user.ChangeRole(parsedNewRole);
            userRepository.Update(user);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<Guid>.Success(user.Id);
        }
    }
}
