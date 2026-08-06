using Application.Common.Features.Hotels.Queries.SearchHotels;
using Application.Common.Models;
using MediatR;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleUI.Screens.Hotels
{
    public class SearchHotelScreen(IMediator mediator)
    {
        private string? _city = null;
        private string? _country = null;
        private double? _minRating = null;

        private int _currentPage = 1;
        private readonly int _pageSize = 5; 
        private string _sortBy = "Name"; 
        private bool _isDescending = false;

        public async Task ShowAsync()
        {
            bool isInside = true;

            while (isInside)
            {
                Console.Clear();
                AnsiConsole.Write(new Rule("[bold]ПОИСК И БРОНИРОВАНИЕ ОТЕЛЕЙ[/]"));
                AnsiConsole.WriteLine();

                var query = new SearchHotelsQuery(
                    City: _city,
                    Country: _country,
                    MinRating: _minRating,
                    SortBy: _sortBy,
                    IsDescending: _isDescending,
                    Page: _currentPage,
                    PageSize: _pageSize
                );

                PagedResultDto<HotelDto>? pagedResult = null;
                string? errorMessage = null;

                await AnsiConsole.Status()
                    .Spinner(Spinner.Known.Dots)
                    .StartAsync("[yellow]Поиск...[/]", async ctx =>
                    {
                        try
                        {
                            var result = await mediator.Send(query);
                            if (result.IsSuccess)
                            {
                                pagedResult = result.Value;
                            }
                            else
                            {
                                errorMessage = result.ErrorMessage;
                            }
                        }
                        catch (Exception ex)
                        {
                            errorMessage = ex.Message;
                        }
                    });

                if (!string.IsNullOrEmpty(errorMessage))
                {
                    AnsiConsole.MarkupLine($"\n[red]Ошибка при поиске отелей:[/] {errorMessage}");
                    ResetFilters();
                    AnsiConsole.MarkupLine("[yellow]Нажмите любую клавишу...[/]");
                    await Task.Run(() => Console.ReadKey(true));
                    continue;
                }

                RenderActiveFilters();

                if (pagedResult == null || !pagedResult.Items.Any())
                {
                    AnsiConsole.MarkupLine("[yellow]По вашему запросу ничего не найдено[/]\n");
                }
                else
                {
                    RenderHotelsTable(pagedResult.Items);
                    AnsiConsole.MarkupLine($"[grey]Страница {_currentPage} из {pagedResult.TotalPages} (Всего найдено отелей: {pagedResult.TotalCount})[/]\n");
                }

                var menu = new SelectionPrompt<string>();

                if (_currentPage < pagedResult?.TotalPages)
                    menu.AddChoice("[green]-> Следующая страница[/]");
                if (_currentPage > 1)
                    menu.AddChoice("[green]<- Предыдущая страница[/]");

                menu.AddChoices(
                    "Настроить фильтры",
                    "Изменить сортировку",
                    "Сбросить все фильтры",
                    "Вернуться в главное меню"
                );

                var choice = AnsiConsole.Prompt(menu);

                switch (choice)
                {
                    case "[green]-> Следующая страница[/]":
                        _currentPage++;
                        break;
                    case "[green]<- Предыдущая страница[/]":
                        _currentPage--;
                        break;
                    case "Настроить фильтры":
                        ConfigureFilters();
                        _currentPage = 1; 
                        break;
                    case "Изменить сортировку":
                        ConfigureSorting();
                        break;
                    case "Сбросить все фильтры":
                        ResetFilters();
                        break;
                    case "Вернуться в главное меню":
                        isInside = false;
                        break;
                }
            }
        }

        private void RenderActiveFilters()
        {
            var filterInfo = new List<string>();
            if (!string.IsNullOrEmpty(_country)) filterInfo.Add($"Страна: [cyan]{_country}[/]");
            if (!string.IsNullOrEmpty(_city)) filterInfo.Add($"Город: [cyan]{_city}[/]");
            if (_minRating.HasValue) filterInfo.Add($"Рейтинг от: [yellow]★ {_minRating:F1}[/]");

            var filterText = filterInfo.Count > 0
                ? string.Join(" | ", filterInfo)
                : "[grey]Нет фильтров[/]";

            AnsiConsole.Write(new Panel(filterText) { Header = new PanelHeader("Фильтры"), Border = BoxBorder.Rounded });
            AnsiConsole.WriteLine();
        }

        private void RenderHotelsTable(IEnumerable<HotelDto> hotels)
        {
            var table = new Table().Border(TableBorder.Rounded).Expand();
            table.AddColumn("[bold]ID отеля[/]");
            table.AddColumn("[bold]Название[/]");
            table.AddColumn("[bold]Локация[/]");
            table.AddColumn("[bold]Адрес[/]");
            table.AddColumn("[bold]Рейтинг[/]");

            foreach (var hotel in hotels)
            {
                table.AddRow(
                    $"[grey]{hotel.Id}[/]",
                    $"[bold blue]{hotel.Name}[/]",
                    $"{hotel.Country}, {hotel.City}",
                    hotel.Address,
                    hotel.Rating.ToString()
                );
            }

            AnsiConsole.Write(table);
        }

       

        private void ConfigureFilters()
        {
            Console.Clear();
            AnsiConsole.Write(new Rule("[yellow]ПАРАМЕТРЫ ФИЛЬТРАЦИИ[/]"));

            _country = AnsiConsole.Prompt(
                new TextPrompt<string>("Введите [blue]страну[/] для поиска (или Enter, чтобы пропустить):")
                    .AllowEmpty());
            if (string.IsNullOrWhiteSpace(_country)) _country = null;

            _city = AnsiConsole.Prompt(
                new TextPrompt<string>("Введите [blue]город[/] для поиска (или Enter, чтобы пропустить):")
                    .AllowEmpty());
            if (string.IsNullOrWhiteSpace(_city)) _city = null;

            var ratingInput = AnsiConsole.Prompt(
                new TextPrompt<string>("Введите [blue]минимальный рейтинг[/] (от 1.0 до 5.0) или Enter для пропуска:")
                    .AllowEmpty()
                    .Validate(input =>
                    {
                        if (string.IsNullOrEmpty(input)) return ValidationResult.Success();

                        if (double.TryParse(input, out var val) && val >= 1.0 && val <= 5.0)
                            return ValidationResult.Success();

                        return ValidationResult.Error("[red]Рейтинг должен быть числом от 1.0 до 5.0![/]");
                    }));

            _minRating = string.IsNullOrEmpty(ratingInput) ? null : double.Parse(ratingInput);
        }

        private void ConfigureSorting()
        {
            var sortChoice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Выберите критерий сортировки:")
                    .AddChoices("По названию", "По рейтингу"));

            _sortBy = sortChoice == "По названию" ? "Name" : "Rating";

            var directionChoice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Порядок отображения:")
                    .AddChoices("Сначала популярные/А-Я", "Сначала низкие/Я-А"));

            _isDescending = directionChoice == "Сначала популярные/А-Я";
        }

        private void ResetFilters()
        {
            _city = null;
            _country = null;
            _minRating = null;
            _currentPage = 1;
            _sortBy = "Name";
            _isDescending = false;
        }
    }
}
