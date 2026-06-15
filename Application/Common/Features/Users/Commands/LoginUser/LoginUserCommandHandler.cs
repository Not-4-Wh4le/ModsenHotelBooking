using Application.Common.Interfaces;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Features.Users.Commands.LoginUser
{
    public class LoginUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork, 
        IMapper mapper) : IRequestHandler<LoginUserCommand, Result<UserSessionDto>>
    {
        public async Task<Result<UserSessionDto>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetUserByUsernameAsync(request.Username, cancellationToken);
            if (user == null)
                return Result<UserSessionDto>.Failure("Invalid username or password");

            bool passwordIsValid = passwordHasher.VerifyPassword(request.Password, user.PasswordHash);
            if(!passwordIsValid)
                return Result<UserSessionDto>.Failure("Invalid username or password");
            var dto = mapper.Map<UserSessionDto>(user);
            await unitOfWork.SaveChangesAsync(cancellationToken); 

            return Result<UserSessionDto>.Success(dto);
        }
    }
}
