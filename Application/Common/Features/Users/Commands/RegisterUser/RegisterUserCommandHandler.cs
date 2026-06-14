using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Features.Users.Commands.RegisterUser
{
    public class RegisterUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork)
        : IRequestHandler<RegisterUserCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetUserByEmailAsync(request.Email, cancellationToken);
            if (user != null)
                return Result<Guid>.Failure("User with this email already exists");

            user = await userRepository.GetUserByUsernameAsync(request.Username, cancellationToken);
            if (user != null)
                return Result<Guid>.Failure("User with this username already exists");

            var passwordHash = passwordHasher.HashPassword(request.Password);
            try
            {
                user = new User(Guid.NewGuid(), request.Username, request.Email, passwordHash);
            }
            catch (Exception ex)
            {
                return Result<Guid>.Failure(ex.Message);
            }

            await userRepository.AddAsync(user, cancellationToken);

            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<Guid>.Success(user.Id);
    }
}
}
