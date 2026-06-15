using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Features.Hotels.Queries.SearchHotels
{
    public class HotelDtoProfile : Profile
    {
        public HotelDtoProfile()
        {
            CreateMap<Hotel, HotelDto>();
        }
    }
}
