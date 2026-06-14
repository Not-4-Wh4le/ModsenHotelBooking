using Application.Common.Interfaces;
using Application.Common.Models;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Features.Users.Queries.GetUsersList
{
    public class GetUsersListQueryHandler(
        IUserRepository userRepository,
        IMapper mapper)
        : IRequestHandler<GetUsersListQuery, Result<PagedResultDto<UserDto>>>

    {
        public async Task<Result<PagedResultDto<UserDto>>> Handle(GetUsersListQuery request, CancellationToken cancellationToken)
        {
            var (users, totalCount) = await userRepository.GetPagedAsync(request.Page, request.PageSize, cancellationToken);
            var dtos = mapper.Map<IReadOnlyList<UserDto>>(users);

            var result = new PagedResultDto<UserDto>(dtos, totalCount, request.Page, request.PageSize);

            return Result<PagedResultDto<UserDto>>.Success(result);
        }
    }
}
