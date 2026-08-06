using Application.Common.Features.Rooms.Queries.SearchRooms;
using Application.Common.Models;
using MediatR;
using Microsoft.IdentityModel.Tokens.Experimental;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleUI.Screens.Rooms
{
    public class RoomCatalogScreen(IMediator mediator)
    {
        private Guid? _hotelId = null;
        private string? _roomType = null;
        private int? _capacity = null;
        private decimal _minPrice = 0;
        private decimal? _maxPrice = null;
        private string? _amenitySearch = null;
        private DateTimeOffset? _checkIn = null;
        private DateTimeOffset? _checkOut = null;

        private int _currentPage = 1;
        private readonly int _pageSize = 5; 
        private string _sortBy = "Price";
        private bool _isDescending = false;

        public async Task ShowAsync()
        {
            bool isInside = true;

            while (isInside)
            {
                Console.Clear();
                AnsiConsole.Write(new Rule("[bold]КАТАЛОГ НОМЕРОВ[/]"));

                var query = new SearchRoomsQuery(
                    HotelId: _hotelId,
                    Type: _roomType,
                    Capacity: _capacity,
                    MinPrice: _minPrice,
                    MaxPrice: _maxPrice,
                    AmenitySearch: _amenitySearch,
                    CheckIn: _checkIn,
                    CheckOut: _checkOut,
                    Page: _currentPage,
                    PageSize: _pageSize,
                    SortBy: _sortBy,
                    IsDescending: _isDescending
                );

                PagedResultDto<RoomCatalogDto>? pagedResult = null;

                await AnsiConsole.Status()
                    .Spinner(Spinner.Known.Dots)
                    .StartAsync("[yellow]Загрузка комнат из каталога...[/]", async ctx =>
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
                                
                            }
                        }
                        catch (Exception ex)
                        {
                            AnsiConsole.MarkupLine($"\n[red]Ошбка:[/] {ex.Message}");
                            _checkIn = null;
                            _checkOut = null;
                            AnsiConsole.MarkupLine("\n[grey]Нажмите любую клавишу, чтобы продолжить...[/]");
                            await Task.Run(() => Console.ReadKey(true));
                        }
                    });

                RenderActiveFiltersPanel();

                if (pagedResult == null || !pagedResult.Items.Any())
                {
                    AnsiConsole.MarkupLine("[yellow]Нет комнат, соответствующих заданным критериям[/]\n");
                }
                else
                {
                    RenderRoomsTable(pagedResult.Items);
                    AnsiConsole.MarkupLine($"[grey]Страница {_currentPage} из {pagedResult.TotalPages} (Всего номеров: {pagedResult.TotalCount})[/]\n");
                }

                var menu = new SelectionPrompt<string>();

                if (_currentPage < pagedResult?.TotalPages)
                    menu.AddChoice("[green]-> Следующая страница[/]");
                if (_currentPage > 1)
                    menu.AddChoice("[green]<- Предыдущая страница[/]");

                menu.AddChoices(
                    "Настроить фильтры",
                    "Указать даты поездки",
                    "Изменить сортировку",
                    "Сбросить все фильтры",
                    "Назад в меню"
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
                    case "Указать даты поездки":
                        ConfigureDates();
                        _currentPage = 1;
                        break;
                    case "Изменить сортировку":
                        ConfigureSorting();
                        break;
                    case "Сбросить все фильтры":
                        ResetFilters();
                        break;
                    case "Назад в меню":
                        isInside = false;
                        break;
                }
            }
        }

        private void RenderActiveFiltersPanel()
        {
            var filterInfo = new List<string>();
            if (!string.IsNullOrEmpty(_roomType)) filterInfo.Add($"Тип: [cyan]{_roomType}[/]");
            if (_capacity.HasValue) filterInfo.Add($"Мест: [cyan]{_capacity}[/]");
            if (_minPrice > 0 || _maxPrice.HasValue) filterInfo.Add($"Цена: [cyan]{_minPrice} - {_maxPrice?.ToString() ?? "∞"}[/]");
            if (!string.IsNullOrEmpty(_amenitySearch)) filterInfo.Add($"Удобства: [cyan]*{_amenitySearch}*[/]");
            if (_checkIn.HasValue && _checkOut.HasValue) filterInfo.Add($"Даты: [yellow]{_checkIn:dd.MM.yyyy} — {_checkOut:dd.MM.yyyy}[/]");

            var filterText = filterInfo.Count > 0 ? string.Join(" | ", filterInfo) : "[grey]Нет активных фильтров[/]";

            AnsiConsole.Write(new Panel(filterText) { Header = new PanelHeader("Фильтры"), Border = BoxBorder.Rounded });
            AnsiConsole.WriteLine();
        }

        private void RenderRoomsTable(IEnumerable<RoomCatalogDto> rooms)
        {
            var table = new Table().Border(TableBorder.Rounded).Expand();
            table.AddColumn("[bold]ID комнаты[/]");
            table.AddColumn("[bold]Отель / Город[/]");
            table.AddColumn("[bold]Тип номера[/]");
            table.AddColumn("[bold]Мест[/]");
            table.AddColumn("[bold]Цена / Ночь[/]");
            table.AddColumn("[bold]Удобства[/]");

            foreach (var room in rooms)
            {
                table.AddRow(
                    room.Id.ToString(), 
                    $"[blue]{room.HotelName}[/]\n[grey]{room.HotelCity}[/]",
                    room.Type,
                    room.Capacity.ToString(),
                    $"[green]{room.PricePerNight:C}[/]",
                    room.Amenities
                );
            }

            AnsiConsole.Write(table);
        }

        private void ConfigureFilters()
        {
            Console.Clear();
            AnsiConsole.Write(new Rule("[yellow]НАСТРОЙКА ФИЛЬТРОВ[/]"));

            _roomType = AnsiConsole.Prompt(
                new TextPrompt<string>("Введите [blue]тип номера[/] (Standard, Suite, Deluxe) или Enter для пропуска:")
                    .AllowEmpty());
            if (string.IsNullOrWhiteSpace(_roomType)) _roomType = null;

            var capacityInput = AnsiConsole.Prompt(
                new TextPrompt<string>("Введите [blue]количество мест[/] или Enter для пропуска:")
                    .AllowEmpty()
                    .Validate(i => string.IsNullOrEmpty(i) || int.TryParse(i, out var res) && res > 0
                        ? ValidationResult.Success() : ValidationResult.Error("[red]Введите число больше 0[/]")));
            _capacity = string.IsNullOrEmpty(capacityInput) ? null : int.Parse(capacityInput);

            var minPriceInput = AnsiConsole.Prompt(
                new TextPrompt<string>("Введите [blue]минимальную цену[/] или Enter для пропуска (0):")
                    .AllowEmpty()
                    .DefaultValue("0"));
            _minPrice = decimal.TryParse(minPriceInput, out var minP) ? minP : 0;

            var maxPriceInput = AnsiConsole.Prompt(
                new TextPrompt<string>("Введите [blue]максимальную цену[/] или Enter для пропуска:")
                    .AllowEmpty());
            _maxPrice = decimal.TryParse(maxPriceInput, out var maxP) ? maxP : null;

            _amenitySearch = AnsiConsole.Prompt(
                new TextPrompt<string>("Поиск по [blue]удобствам[/] (подстрока, например 'Wi-Fi') или Enter для пропуска:")
                    .AllowEmpty());
            if (string.IsNullOrWhiteSpace(_amenitySearch)) _amenitySearch = null;
        }

        private void ConfigureDates()
        {
            Console.Clear();
            AnsiConsole.Write(new Rule("[yellow]ВВОД ДАТ ДЛЯ ПРОВЕРКИ ДОСТУПНОСТИ[/]"));

            var checkInInput = AnsiConsole.Prompt(
                new TextPrompt<string>("Введите [green]дата заезда[/] (дд.мм.гггг) или Enter для сброса:")
                    .AllowEmpty()
                    .Validate(s => string.IsNullOrEmpty(s) || DateTimeOffset.TryParse(s, out _)
                        ? ValidationResult.Success() : ValidationResult.Error("[red]Неверный формат даты![/]")));

            if (string.IsNullOrEmpty(checkInInput))
            {
                _checkIn = null;
                _checkOut = null;
                return;
            }

            _checkIn = DateTimeOffset.Parse(checkInInput);

            var checkOutInput = AnsiConsole.Prompt(
                new TextPrompt<string>("Введите [green]дата выезда[/] (дд.мм.гггг):")
                    .Validate(s => DateTimeOffset.TryParse(s, out var d) && d > _checkIn
                        ? ValidationResult.Success() : ValidationResult.Error("[red]Дата выезда должна быть позже даты заезда![/]")));

            _checkOut = DateTimeOffset.Parse(checkOutInput);
        }

        private void ConfigureSorting()
        {
            _sortBy = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Сортировать по:")
                    .AddChoices("Price", "Capacity"));

            var direction = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Направление сортировки:")
                    .AddChoices("По возрастанию", "По убыванию"));

            _isDescending = direction == "По убыванию";
        }

        private void ResetFilters()
        {
            _hotelId = null;
            _roomType = null;
            _capacity = null;
            _minPrice = 0;
            _maxPrice = null;
            _amenitySearch = null;
            _checkIn = null;
            _checkOut = null;
            _currentPage = 1;
            _sortBy = "Price";
            _isDescending = false;
        }
    }
}
