using Application.Common.Features.Users.Commands.LoginUser;
using Application.Common.Features.Users.Commands.RegisterUser;
using Application.Common.Interfaces;
using ConsoleUI.Screens;
using ConsoleUI.Screens.Admin;
using ConsoleUI.Screens.Customer;
using ConsoleUI.Screens.HotelManager;
using ConsoleUI.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleUI
{
    public class ConsoleAppEngine(
        CurrentUserService currentUser,
        AdminMenuScreen adminMenuScreen,
        CustomerMenuScreen customerMenuScreen,
        HotelManagerMenuScreen hotelManagerMenuScreen,
        GuestMenuScreen guestMenuScreen) 
    {
        private readonly Dictionary<string, Func<Task>> roleScreens = new(StringComparer.OrdinalIgnoreCase)
        {
            { "Admin", adminMenuScreen.ShowAsync },
            { "Customer", customerMenuScreen.ShowAsync },
            { "HotelManager", hotelManagerMenuScreen.ShowAsync }
        };

        public async Task RunAsync()
        {
            bool isRunning = true;
            while (isRunning)
            {
                try
                {
                    if (!currentUser.IsAuthenticated)
                    {
                        await guestMenuScreen.ShowAsync();

                        if(!currentUser.IsAuthenticated)
                        {
                            isRunning = false;
                            continue;
                        }
                    }
                    else
                    {
                        var role = currentUser.Role;
                        if (role != null && roleScreens.TryGetValue(role, out var showScreenAsync))
                            await showScreenAsync();
                        else
                        {
                            AnsiConsole.MarkupLine("Роль не найдена");
                            currentUser.Logout();
                        }
                    }
                }
                catch (Exception ex)
                {
                    AnsiConsole.Write(new Panel($"[red]Ошибка:[/] {ex.Message}")
                        .Header("[bold red] Системный сбой [/]")
                        .Border(BoxBorder.Rounded)
                        .BorderColor(Color.Red));

                    AnsiConsole.MarkupLine("\n[grey]Нажмите любую клавишу для продолжения...[/]");
                    Console.ReadKey(true);
                }
            }
        }
    }
}
