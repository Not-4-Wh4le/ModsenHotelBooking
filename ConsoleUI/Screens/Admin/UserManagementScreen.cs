using ConsoleUI.Screens.Admin.UserManagementScreens;
using MediatR;
using ModsenHotelBooking.ConsoleUI.Screens.Admin;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleUI.Screens.Admin
{
    public class UserManagementScreen(
        ViewUsersListScreen viewUsersListScreen,
        AddManagerScreen addManagerScreen) : BaseMenuScreen
    {
        protected override List<(string Title, Func<Task> Action)> ConfMenu() => [
            ("Показать список всех пользователей", viewUsersListScreen.ShowAsync),
            ("Добавить нового менеджера", addManagerScreen.ShowAsync)];

        protected override string GetHeader() => "[blue]УПРАВЛЕНИЕ ПОЛЬЗОВАТЕЛЯМИ[/]";
    }
}
