using Application.DiscountFactory;
using Application.DiscountFactory.Interfaces;
using Domain.Strategies;
using Application.Common.Behavior;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Text;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();

            services.AddScoped<IDiscountStrategyFactory, LoyaltyDiscountStrategyFactory>();
            services.AddScoped<IDiscountStrategyFactory, PromoCodeDiscountStrategyFactory>();
            services.AddScoped<IDiscountStrategyFactory, EmptyDicsountStrategyFactory>();
            services.AddScoped<IDiscountStrategyResolver, DiscountStrategyResolver>();

            
            services.AddMediatR(conf =>
            {
                conf.RegisterServicesFromAssemblies(assembly);
                conf.AddBehavior(typeof(IPipelineBehavior<,>), typeof(AuthorizeBehavior<,>));
            });

            services.AddAutoMapper(conf => conf.AddMaps(assembly));

            services.AddValidatorsFromAssembly(assembly);

            return services;

        }
    }
}
