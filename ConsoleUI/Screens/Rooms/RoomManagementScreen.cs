using ConsoleUI.Screens.Hotels;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleUI.Screens.Rooms
{
    public class RoomManagementScreen(
         RoomCatalogScreen roomCatalogScreen,
         UpdateRoomScreen updateRoomScreen,
         CreateRoomScreen createRoomScreen)
    {
        public async Task ShowAsync()
        {
            bool isInside = true;

            while (isInside)
            {
                Console.Clear();
                AnsiConsole.Write(new Rule("[blue]УПРАВЛЕНИЕ НОМЕРАМИ[/]"));

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .AddChoices(
                        "Список номеров",
                        "Добавить номер",
                        "Изменить номер",
                        "Удалить номер",
                        "Вернуться в главное меню"));

                switch (choice)
                {
                    case "Список номеров":
                        await roomCatalogScreen.ShowAsync();
                        break;
                    case "Добавить номер":
                        await createRoomScreen.ShowAsync();
                        break;
                    case "Изменить номер":
                        await updateRoomScreen.ShowAsync();
                        break;
                    case "Удалить номер":
                        break;
                    case "Вернуться в главное меню":
                        isInside = false;
                        break;
                }
            }
        }

    }
}
