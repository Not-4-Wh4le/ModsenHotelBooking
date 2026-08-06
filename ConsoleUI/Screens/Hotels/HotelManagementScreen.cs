using ConsoleUI.Screens.Admin.UserManagementScreens;
using ModsenHotelBooking.ConsoleUI.Screens.Admin;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleUI.Screens.Hotels
{
    public class HotelManagementScreen(
        HotelListScreen hotelListScreen,
        CreateHotelScreen createHotelScreen,
        UpdateHotelScreen updateHotelScreen)
    {
        public async Task ShowAsync()
        {
            bool isInside = true;

            while (isInside)
            {
                Console.Clear();
                AnsiConsole.Write(new Rule("[blue]УПРАВЛЕНИЕ ОТЕЛЯМИ[/]"));

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .AddChoices(
                        "Список отелей", 
                        "Добавить новый отель", 
                        "Изменить отель", 
                        "Удалить отель",
                        "Вернуться в главное меню"));

                switch (choice)
                {
                    case "Список отелей":
                        await hotelListScreen.ShowAsync();
                        break;
                    case "Добавить новый отель":
                        await createHotelScreen.ShowAsync();
                        break;
                    case "Изменить отель":
                        await updateHotelScreen.ShowAsync();
                        break;
                    case "Удалить отель":
                        break;
                    case "Вернуться в главное меню":
                        isInside = false;
                        break;
                }
            }
        }

    }
}
