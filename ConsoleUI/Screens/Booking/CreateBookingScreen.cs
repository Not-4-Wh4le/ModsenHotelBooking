using Application.Common.Features.Bookings.Commands.CreateBooking;
using Application.Common.Interfaces;
using ConsoleUI.Services;
using MediatR;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleUI.Screens.Booking
{
    public class CreateBookingScreen(
        CurrentUserService currentUserService,
        IMediator mediator)
    {
        public async Task ShowAsync()
        {
            Console.Clear();
            AnsiConsole.Write(new Rule("[bold]БРОНИРОВАНИЕ[/]"));
            AnsiConsole.WriteLine();

            if (!currentUserService.IsAuthenticated || !currentUserService.Id.HasValue)
            {
                AnsiConsole.MarkupLine("[red]Ошибка:[/] Нужна авторизация");
                await WaitForKeyPressAsync();
                return;
            }

            AnsiConsole.MarkupLine($"Клиент: [bold cyan]{currentUserService.Username}[/]");
            AnsiConsole.WriteLine();

            var roomIdInput = AnsiConsole.Prompt(
                new TextPrompt<string>("ID номера:")
                    .Validate(input => Guid.TryParse(input, out _)
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Неверный формат Guid[/]")));

            Guid roomId = Guid.Parse(roomIdInput);

            var checkInInput = AnsiConsole.Prompt(
                new TextPrompt<string>("Дата заезда (дд.мм.гггг):")
                    .Validate(s => DateTimeOffset.TryParse(s, out var d) && d.Date >= DateTimeOffset.UtcNow.Date
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Дата в прошлом[/]")));

            DateTimeOffset checkInDate = DateTimeOffset.Parse(checkInInput);

            var checkOutInput = AnsiConsole.Prompt(
                new TextPrompt<string>("Дата выезда (дд.мм.гггг):")
                    .Validate(s => DateTimeOffset.TryParse(s, out var d) && d.Date > checkInDate.Date
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Дата выезда должна быть позже даты заезда[/]")));

            DateTimeOffset checkOutDate = DateTimeOffset.Parse(checkOutInput);

            var promoCode = AnsiConsole.Prompt(
                new TextPrompt<string>("Промокод (Enter для пропуска):")
                    .AllowEmpty());

            if (string.IsNullOrWhiteSpace(promoCode)) promoCode = null;

            bool useLoyaltyPoints = AnsiConsole.Confirm("Использовать баллы лояльности", defaultValue: false);

            AnsiConsole.WriteLine();

            if (!AnsiConsole.Confirm("Подтвердить бронирование", defaultValue: true))
            {
                AnsiConsole.MarkupLine("[yellow]Отменено[/]");
                await WaitForKeyPressAsync();
                return;
            }

            var command = new CreateBookingCommand(
                UserId: currentUserService.Id.Value,
                RoomId: roomId,
                CheckInDate: checkInDate,
                CheckOutDate: checkOutDate,
                PromoCode: promoCode,
                UseLoyaltyPoints: useLoyaltyPoints
            );

            await AnsiConsole.Status()
                .Spinner(Spinner.Known.Dots)
                .StartAsync("[yellow]Оформление...[/]", async ctx =>
                {
                    try
                    {
                        var result = await mediator.Send(command);

                        if (result.IsSuccess)
                        {
                            AnsiConsole.MarkupLine("\n[green]Бронирование создано[/]");
                            AnsiConsole.MarkupLine($"ID брони: [bold]{result.Value}[/]");
                        }
                        else
                        {
                            AnsiConsole.MarkupLine($"\n[red]Ошибка:[/] {result.ErrorMessage}");
                        }
                    }
                    catch (Exception ex)
                    {
                        AnsiConsole.MarkupLine($"\n[red]Ошибка бэкенда:[/] {ex.Message}");
                    }
                });

            await WaitForKeyPressAsync();
        }

        private static async Task WaitForKeyPressAsync()
        {
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[grey]Нажмите клавишу для возврата[/]");
            await Task.Run(() => Console.ReadKey(true));
        }
    }
}
