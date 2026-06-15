using Application.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Features.Users.Commands.DeleteUser
{
    public class DeleteUserCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser
        ) : IRequestHandler<DeleteUserCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetUserByUsernameAsync(request.Username, cancellationToken);
            if (user == null)
                return Result<Guid>.Failure("User with this username does not exist");
            
            if(user.IsDeleted)
                return Result<Guid>.Failure("User is already deleted");
            
            if(user.Id == currentUser.Id)
                return Result<Guid>.Failure("You do not have permission to delete this user");

            user.DeleteUser();
            userRepository.Update(user);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<Guid>.Success(user.Id);
        }
    }
}
