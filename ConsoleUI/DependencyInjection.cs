using Application.Common.Interfaces;
using ConsoleUI.Screens;
using ConsoleUI.Screens.Admin;
using ConsoleUI.Screens.Admin.UserManagementScreens;
using ConsoleUI.Screens.Booking;
using ConsoleUI.Screens.Customer;
using ConsoleUI.Screens.HotelManager;
using ConsoleUI.Screens.Hotels;
using ConsoleUI.Screens.Rooms;
using ConsoleUI.Services;
using Microsoft.Extensions.DependencyInjection;
using ModsenHotelBooking.ConsoleUI.Screens.Admin;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleUI
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddUi(this IServiceCollection services)
        {
            services.AddScoped<CurrentUserService>();

            services.AddScoped<ICurrentUserService>(sp => sp.GetRequiredService<CurrentUserService>());

            services.AddTransient<СustomerBookingHistoryScreen>();
            services.AddTransient<CreateBookingScreen>();

            services.AddTransient<RoomManagementScreen>();
            services.AddTransient<UpdateRoomScreen>();
            services.AddTransient<CreateRoomScreen>();
            services.AddTransient<RoomCatalogScreen>();


            services.AddTransient<SearchHotelScreen>();
            services.AddTransient<HotelListScreen>();
            services.AddTransient<HotelManagementScreen>();
            services.AddTransient<CreateHotelScreen>();
            services.AddTransient<UpdateHotelScreen>();
            //Admin
            services.AddTransient<ViewUsersListScreen>();
            services.AddTransient<AddManagerScreen>();
            services.AddTransient<UserManagementScreen>();
            services.AddTransient<AdminMenuScreen>();
            


            //Customer
            services.AddTransient<CustomerMenuScreen>();


            services.AddTransient<HotelManagerMenuScreen>();

            services.AddTransient<RegisterScreen>();
            services.AddTransient<LoginScreen>();
            services.AddTransient<GuestMenuScreen>();




            services.AddTransient<ConsoleAppEngine>();

            return services;
        }
    }
}
