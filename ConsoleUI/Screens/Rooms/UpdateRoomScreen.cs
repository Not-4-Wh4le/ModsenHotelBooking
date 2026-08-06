using Application.Common.Features.Rooms.Commands.UpdateRoom;
using Domain.Enums;
using MediatR;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleUI.Screens.Rooms
{
    public class UpdateRoomScreen(IMediator mediator)
    {
        public async Task ShowAsync()
        {
            Console.Clear();
            AnsiConsole.Write(new Rule("[bold]РЕДАКТИРОВАНИЕ НОМЕРА[/]"));
            AnsiConsole.WriteLine();

            var roomIdInput = AnsiConsole.Prompt(
                new TextPrompt<string>("Вставьте [yellow]ID комнаты[/], которую хотите изменить:")
                    .Validate(input => Guid.TryParse(input, out _)
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Неверный формат Guid! Скопируйте корректный ID из каталога номеров.[/]")));

            Guid roomId = Guid.Parse(roomIdInput);

            var number = AnsiConsole.Prompt(
                new TextPrompt<int>("Введите [blue]новый номер[/] комнаты:")
                    .Validate(n => n > 0
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Номер комнаты должен быть больше нуля[/]")));

            var roomTypes = Enum.GetNames(typeof(RoomType));
            var roomType = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Выберите [blue]новый тип[/] номера:")
                    .AddChoices(roomTypes));

            var capacity = AnsiConsole.Prompt(
                new TextPrompt<int>("Введите [blue]новую вместимость[/] (кол-во человек):")
                    .Validate(c => c > 0
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Вместимость должна быть положительным числом[/]")));

            var pricePerNight = AnsiConsole.Prompt(
                new TextPrompt<decimal>("Введите [blue]новую цену[/] за одну ночь:")
                    .Validate(p => p > 0
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Цена должна быть больше нуля[/]")));

            var description = AnsiConsole.Prompt(
                new TextPrompt<string>("Введите [blue]новое описание[/] номера (до 500 симв.):")
                    .Validate(d => d.Length <= 500
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Описание слишком длинное[/]")));

            var amenities = AnsiConsole.Prompt(
                new TextPrompt<string>("Введите [blue]новые удобства[/] через запятую (до 250 симв.):")
                    .Validate(a => a.Length <= 250
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Строка удобств слишком длинная[/]")));

            AnsiConsole.WriteLine();

            if (!AnsiConsole.Confirm("Вы уверены, что хотите сохранить изменения для этого номера?"))
            {
                AnsiConsole.MarkupLine("[yellow]Обновление отменено пользователем.[/]");
                await WaitForKeyPressAsync();
                return;
            }

            var command = new UpdateRoomCommand(
                Id: roomId,
                Number: number,
                RoomType: roomType,
                Capacity: capacity,
                PricePerNight: pricePerNight,
                Description: description,
                Amenities: amenities
            );

            await AnsiConsole.Status()
                .Spinner(Spinner.Known.Dots)
                .StartAsync("[yellow]Обновление...[/]", async ctx =>
                {
                    var result = await mediator.Send(command);

                    if (result.IsSuccess)
                    {
                        AnsiConsole.MarkupLine($"\n[green]Данные номера успешно обновлены![/] ID: [bold]{result.Value}[/]");
                    }
                    else
                    {
                        AnsiConsole.MarkupLine($"\n[red]Ошибка при обновлении номера:[/] {result.ErrorMessage}");
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
