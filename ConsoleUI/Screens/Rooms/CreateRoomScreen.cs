using Application.Common.Features.Rooms.Commands.CreateRoom;
using Domain.Enums;
using MediatR;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleUI.Screens.Rooms
{
    public class CreateRoomScreen(IMediator mediator)
    {
        public async Task ShowAsync()
        {
            Console.Clear();
            AnsiConsole.Write(new Rule("[bold]ДОБАВЛЕНИЕ НОВОГО НОМЕРА[/]"));
            AnsiConsole.WriteLine();

            var hotelIdInput = AnsiConsole.Prompt(
                new TextPrompt<string>("Вставьте [blue]ID отеля[/], к которому принадлежит номер:")
                    .Validate(input => Guid.TryParse(input, out _)
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Неверный формат Guid")));

            Guid hotelId = Guid.Parse(hotelIdInput);

            var number = AnsiConsole.Prompt(
                new TextPrompt<int>("Введите [blue]номер комнаты[/]:")
                    .Validate(n => n > 0
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Номер комнаты должен быть больше нуля[/]")));

            var roomTypes = Enum.GetNames(typeof(RoomType));
            var roomType = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Выберите [blue]тип номера[/]:")
                    .AddChoices(roomTypes));

            var capacity = AnsiConsole.Prompt(
                new TextPrompt<int>("Введите [blue]вместимость[/] (кол-во человек):")
                    .Validate(c => c > 0
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Вместимость должна быть положительным числом[/]")));

            var area = AnsiConsole.Prompt(
                new TextPrompt<int>("Введите [blue]площадь номера[/] (кв. м):")
                    .Validate(a => a > 0
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Площадь должна быть больше нуля[/]")));

            var pricePerNight = AnsiConsole.Prompt(
                new TextPrompt<decimal>("Введите [blue]цену за одну ночь[/]:")
                    .Validate(p => p > 0
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Цена должна быть больше нуля[/]")));

            var description = AnsiConsole.Prompt(
                new TextPrompt<string>("Введите [blue]описание номера[/] (до 500 симв.):")
                    .Validate(d => d.Length <= 500
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Описание слишком длинное")));

            var amenities = AnsiConsole.Prompt(
                new TextPrompt<string>("Введите [blue]удобства[/] через запятую (например: Wi-Fi, TV, AC):")
                    .Validate(a => a.Length <= 250
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Строка удобств слишком длинная[/]")));

            AnsiConsole.WriteLine();

            if (!AnsiConsole.Confirm("Вы уверены, что хотите добавить этот номер в отель?"))
            {
                AnsiConsole.MarkupLine("[yellow]Операция отменена пользователем[/]");
                await WaitForKeyPressAsync();
                return;
            }

            var command = new CreateRoomCommand(
                Number: number,
                HotelId: hotelId,
                RoomType: roomType,
                Capacity: capacity,
                PricePerNight: pricePerNight,
                Description: description,
                Amenities: amenities,
                Area: area
            );

            await AnsiConsole.Status()
                .Spinner(Spinner.Known.Dots)
                .StartAsync("[yellow]Сохранение...[/]", async ctx =>
                {
                    var result = await mediator.Send(command);

                    if (result.IsSuccess)
                    {
                        AnsiConsole.MarkupLine($"\n[green]Номер №{number} успешно добавлен![/] ID: [bold]{result.Value}[/]");
                    }
                    else
                    {
                        AnsiConsole.MarkupLine($"\n[red]Ошибка при создании номера:[/] {result.ErrorMessage}");
                    }
                });

            await WaitForKeyPressAsync();
        }

        private static async Task WaitForKeyPressAsync()
        {
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[grey]Нажмите любую клавишу для возврата в меню...[/]");
            await Task.Run(() => Console.ReadKey(true));
        }
    }
}
