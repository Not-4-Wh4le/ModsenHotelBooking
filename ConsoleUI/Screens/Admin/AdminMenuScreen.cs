using ConsoleUI.Screens.Booking;
using ConsoleUI.Screens.Hotels;
using ConsoleUI.Screens.Rooms;
using ConsoleUI.Services;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleUI.Screens.Admin
{
    public class AdminMenuScreen(
        CurrentUserService currentUser, 
        UserManagementScreen userManagementScreen,
        HotelManagementScreen hotelManagementScreen,
        RoomManagementScreen roomManagementScreen)
        : BaseMenuScreen
    {
        protected override List<(string Title, Func<Task> Action)> ConfMenu() => [
            ("Управление пользователями", userManagementScreen.ShowAsync),
            ("Управление отелями", hotelManagementScreen.ShowAsync),
            ("Управление номерами", roomManagementScreen.ShowAsync)];



        protected override string GetHeader() =>
            $"Вы вошли как: [green]{currentUser.Username}[/] | Роль: [yellow]Админисратор[/]";

        protected override Task OnExitAsync()
        {
            currentUser.Logout();
            return Task.CompletedTask;
        }

        protected override string ExitOptionText => "Выйти";
    }
}

