using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Features.Users.Commands.LoginUser
{
    public class LoginUserProfile : Profile
    {
        public LoginUserProfile()
        {
            CreateMap<User, UserSessionDto>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));
        }
    }
}
