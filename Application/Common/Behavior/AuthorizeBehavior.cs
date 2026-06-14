using Application.Common.Interfaces;
using Application.Common.Security;
using MediatR;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Application.Common.Behavior
{
    public class AuthorizeBehavior<TRequest, TResponse>(
        ICurrentUserService currentUser)
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var authAttribute = request.GetType().GetCustomAttributes<AuthorizeAttribute>();

            if(!authAttribute.Any())
                return await next();

            if (currentUser.Id == null)
                throw new UnauthorizedAccessException("User must be authorized");

            var rolesString = authAttribute
                .Select(a => a.Roles)
                .Where(r => !string.IsNullOrWhiteSpace(r));

            if (rolesString.Any())
            {
                bool isAuthorized = rolesString
                    .SelectMany(roles => roles.Split(","))
                    .Select(role => role.Trim())
                    .Any(requiredRole => requiredRole == currentUser.Role);

                if(!isAuthorized)
                    throw new UnauthorizedAccessException("You do not have permission to perform this action");
            }

            return await next();
        }
    }
}
