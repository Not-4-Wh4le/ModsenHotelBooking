using Application.Common.Features.Hotels.Queries.GetManagedHotels;
using ConsoleUI.Services;
using MediatR;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleUI.Screens.Hotels
{
    public class HotelListScreen(IMediator mediator, CurrentUserService currentUser)
    {
        public async Task ShowAsync()
        {
            int currentPage = 1;
            const int pageSize = 4;
            string currentSort = "Name";
            bool isDescending = false;
            bool isInside = true;

            while (isInside)
            {
                Console.Clear();
                AnsiConsole.Write(new Rule("[blue]СПИСОК ДОСТУПНЫХ ОТЕЛЕЙ[/]"));
                AnsiConsole.WriteLine();

                Guid? managerFilter = currentUser.Role == "HotelManager"
                    ? currentUser.Id
                    : null;

                var query = new GetManagedHotelsQuery(
                    ManagerId: managerFilter,
                    SortBy: currentSort,
                    IsDescending: isDescending,
                    Page: currentPage,
                    PageSize: pageSize
                );

                IReadOnlyList<ManagedHotelsDto> hotels = Array.Empty<ManagedHotelsDto>();
                int totalCount = 0;
                bool isSuccess = false;
                string? errorMessage = null;

                await AnsiConsole.Status()
                    .Spinner(Spinner.Known.Dots)
                    .StartAsync("Загрузка отелей...", async ctx =>
                    {
                        var result = await mediator.Send(query);
                        if (result.IsSuccess)
                        {
                            isSuccess = true;
                            hotels = result.Value.Items;
                            totalCount = result.Value.TotalCount;
                        }
                        else
                        {
                            errorMessage = result.ErrorMessage;
                        }
                    });

                if (!isSuccess)
                {
                    AnsiConsole.Write(new Panel($"[red]Ошибка:[/] {errorMessage}").BorderColor(Color.Red));
                    Console.ReadKey(true);
                    return;
                }

                int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
                if (totalPages == 0) totalPages = 1;

                if (hotels.Count == 0)
                {
                    AnsiConsole.MarkupLine("[yellow]Отели не найдены[/]\n");
                }
                else
                {
                    var table = new Table().Border(TableBorder.Rounded).BorderColor(Color.Blue);
                    var sortIndicator = isDescending ? "▼" : "▲";

                    table.Title($"Отели ({currentPage}/{totalPages}) | Сортировка: [yellow]{currentSort} {sortIndicator}[/]");
                    table.AddColumn("[yellow]Id[/]");
                    table.AddColumn("[yellow]Название[/]");
                    table.AddColumn("[yellow]Страна / Город[/]");
                    table.AddColumn("[yellow]Адрес[/]");
                    table.AddColumn("[yellow]Рейтинг[/]");
                    if (currentUser.Role == "Admin")
                        table.AddColumn("[white]Управляющий[/]");

                    foreach (var hotel in hotels)
                    {
                        var stars = new string('★', (int)Math.Round(hotel.Rating)) + new string('☆', 5 - (int)Math.Round(hotel.Rating));

                        if (currentUser.Role == "Admin")
                            table.AddRow(hotel.Id.ToString(), hotel.Name, $"{hotel.Country}, {hotel.City}", hotel.Address, $"[yellow]{hotel.Rating:F1}[/] {stars}", hotel.ManagerUsername);
                        else
                            table.AddRow(hotel.Id.ToString(), hotel.Name, $"{hotel.Country}, {hotel.City}", hotel.Address, $"[yellow]{hotel.Rating:F1}[/] {stars}");
                    }

                    AnsiConsole.Write(table);
                }

                var menu = new SelectionPrompt<string>().Title("[yellow]Навигация и управление:[/]");

                const string next = "-> Следующая страница";
                const string prev = "<- Предыдущая страница";
                const string toggleSortName = "Сортировать по названию";
                const string toggleSortRating = "Сортировать по рейтингу";
                const string exit = "Назад";

                if (currentPage < totalPages) menu.AddChoice(next);
                if (currentPage > 1) menu.AddChoice(prev);

                menu.AddChoice(toggleSortName);
                menu.AddChoice(toggleSortRating);
                menu.AddChoice(exit);

                var choice = AnsiConsole.Prompt(menu);

                switch (choice)
                {
                    case next: currentPage++; break;
                    case prev: currentPage--; break;
                    case toggleSortName:
                        if (currentSort == "Name") isDescending = !isDescending;
                        else { currentSort = "Name"; isDescending = false; }
                        currentPage = 1;
                        break;
                    case toggleSortRating:
                        if (currentSort == "Rating") isDescending = !isDescending;
                        else { currentSort = "Rating"; isDescending = true; }
                        currentPage = 1;
                        break;
                    case exit:
                        isInside = false;
                        break;
                }
            }
        }
    }
}
