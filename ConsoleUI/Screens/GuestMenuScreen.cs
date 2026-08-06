using Application.Common.Interfaces;
using ConsoleUI.Services;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleUI.Screens
{
    public class GuestMenuScreen(
        CurrentUserService currentUser, 
        LoginScreen loginScreen,
        RegisterScreen registerScreen) : BaseMenuScreen
    {
        protected override List<(string Title, Func<Task> Action)> ConfMenu() => [
             new ("Войти в систему", loginScreen.ShowAsync),
             new ("Зарегистрироваться", registerScreen.ShowAsync)];

        protected override string GetHeader() => "Modsen Booking";

        protected override string ExitOptionText => "Выход";

        protected override bool ShouldClose() => currentUser.IsAuthenticated;
    }
}
