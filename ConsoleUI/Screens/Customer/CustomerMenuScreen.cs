using ConsoleUI.Screens.Admin;
using ConsoleUI.Screens.Booking;
using ConsoleUI.Screens.Hotels;
using ConsoleUI.Screens.Rooms;
using ConsoleUI.Services;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleUI.Screens.Customer
{
    public class CustomerMenuScreen(
        CurrentUserService currentUser,
        RoomCatalogScreen roomCatalogScreen,
        SearchHotelScreen searchHotelScreen,
        CreateBookingScreen createBookingScreen,
        СustomerBookingHistoryScreen сustomerBookingHistoryScreen)
        : BaseMenuScreen
    {
        protected override List<(string Title, Func<Task> Action)> ConfMenu() => [
            ("Поиск отелей", searchHotelScreen.ShowAsync),
            ("Поиск номеров", roomCatalogScreen.ShowAsync),
            ("Создать бронь", createBookingScreen.ShowAsync),
            ("История бронирований", сustomerBookingHistoryScreen.ShowAsync)];

      

        protected override string GetHeader() =>         
            $"Вы вошли как: [green]{currentUser.Username}[/] | Роль: [yellow]Клиент[/]";

        protected override Task OnExitAsync()
        {
            currentUser.Logout();
            return Task.CompletedTask;
        }

        protected override string ExitOptionText => "Выйти";
    }
}
