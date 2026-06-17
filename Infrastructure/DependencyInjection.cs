using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Repositories;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Security;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            MongoConfiguration.Configure();

            services.AddSingleton<MongoDbContext>();

            services.AddSingleton<IPasswordHasher, PasswordHasher>();

            services.AddScoped<IUserRepository, MongoUserRepository>();
            services.AddScoped<IBookingRepository, MongoBookingRepository>();
            services.AddScoped<ILoyaltyRepository, MongoLoyaltyRepository>();
            services.AddScoped<IRoomRepository, MongoRoomRepository>();
            services.AddScoped<IHotelRepository, MongoHotelRepository>();

            return services;
        }
    }
}
