using Application.Common.Features.Bookings.Queries.GetCustomerBookingHistory;
using Application.Common.Models;
using ConsoleUI.Services;
using MediatR;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleUI.Screens.Booking
{
    public class СustomerBookingHistoryScreen(
        CurrentUserService currentUserService,
        IMediator mediator)
    {
        private int _currentPage = 1;
        private readonly int _pageSize = 5;

        public async Task ShowAsync()
        {
            bool isInside = true;

            while (isInside)
            {
                Console.Clear();
                AnsiConsole.Write(new Rule("[bold]ИСТОРИЯ БРОНИРОВАНИЙ[/]"));
                AnsiConsole.WriteLine();

                if (!currentUserService.IsAuthenticated || !currentUserService.Id.HasValue)
                {
                    AnsiConsole.MarkupLine("[red]Ошибка:[/] Требуется авторизация");
                    await WaitForKeyPressAsync();
                    return;
                }

                var query = new GetCustomerBookingHistoryQuery(
                    CustomerId: currentUserService.Id.Value,
                    Page: _currentPage,
                    PageSize: _pageSize
                );

                PagedResultDto<CustomerBookingHistoryDto>? pagedResult = null;
                string? errorMessage = null;

                await AnsiConsole.Status()
                    .Spinner(Spinner.Known.Dots)
                    .StartAsync("[yellow]Загрузка...[/]", async ctx =>
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
                    AnsiConsole.MarkupLine($"\n[red]Ошибка:[/] {errorMessage}");
                    await WaitForKeyPressAsync();
                    return;
                }

                if (pagedResult == null || !pagedResult.Items.Any())
                {
                    AnsiConsole.MarkupLine("[yellow]История бронирований пуста[/]\n");
                }
                else
                {
                    RenderHistoryTable(pagedResult.Items);
                    AnsiConsole.MarkupLine($"[grey]Страница {_currentPage} из {pagedResult.TotalPages} (Всего: {pagedResult.TotalCount})[/]\n");
                }

                var menu = new SelectionPrompt<string>().Title("Действие:");

                if (_currentPage < pagedResult?.TotalPages)
                    menu.AddChoice("[green]▶ Следующая страница[/]");
                if (_currentPage > 1)
                    menu.AddChoice("[green]◀ Предыдущая страница[/]");

                menu.AddChoice("[red]Назад в меню[/]");

                var choice = AnsiConsole.Prompt(menu);

                switch (choice)
                {
                    case "[green]▶ Следующая страница[/]":
                        _currentPage++;
                        break;
                    case "[green]◀ Предыдущая страница[/]":
                        _currentPage--;
                        break;
                    case "[red]Назад в меню[/]":
                        isInside = false;
                        break;
                }
            }
        }

        private void RenderHistoryTable(IEnumerable<CustomerBookingHistoryDto> bookings)
        {
            var table = new Table().Border(TableBorder.Rounded).Expand();
            table.AddColumn("[bold]ID брони[/]");
            table.AddColumn("[bold]Отель[/]");
            table.AddColumn("[bold]Номер[/]");
            table.AddColumn("[bold]Тип[/]");
            table.AddColumn("[bold]Заезд[/]");
            table.AddColumn("[bold]Выезд[/]");
            table.AddColumn("[bold]Цена[/]");
            table.AddColumn("[bold]Статус[/]");

            foreach (var booking in bookings)
            {
                table.AddRow(
                    $"{booking.Id}",
                    $"{booking.HotelName}",
                    booking.RoomNumber.ToString(),
                    booking.RoomType,
                    $"{booking.CheckInDate:dd.MM.yyyy}",
                    $"{booking.CheckOutDate:dd.MM.yyyy}",
                    $"{booking.FinalPrice}",
                    FormatStatus(booking.Status)
                );
            }

            AnsiConsole.Write(table);
        }

        private string FormatStatus(string status)
        {
            return status.ToLower() switch
            {
                "created" => $"[yellow]{status}[/]",
                "confirmed" => $"[green]{status}[/]",
                "cancelled" => $"[red]{status}[/]",
                _ => status
            };
        }

        private static async Task WaitForKeyPressAsync()
        {
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[grey]Нажмите любую клавишу[/]");
            await Task.Run(() => Console.ReadKey(true));
        }
    }
}
