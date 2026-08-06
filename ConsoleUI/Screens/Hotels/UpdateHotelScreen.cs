using Application.Common.Features.Hotels.Commands.UpdateHotel;
using MediatR;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleUI.Screens.Hotels
{
    public class UpdateHotelScreen(IMediator mediator)
    {
        public async Task ShowAsync()
        {
            Console.Clear();
            AnsiConsole.Write(new Rule("[yellow]РЕДАКТИРОВАНИЕ ОТЕЛЯ[/]"));
            AnsiConsole.WriteLine();

            var hotelIdInput = AnsiConsole.Prompt(
                new TextPrompt<string>("Вставьте ID отеля, который хотите изменить:")
                    .Validate(input => Guid.TryParse(input, out _)
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Неверный формат Guid! Скопируйте корректный ID из списка отелей.[/]")));

            Guid hotelId = Guid.Parse(hotelIdInput);

            var newName = AnsiConsole.Prompt(
                new TextPrompt<string>("Введите новое названиеотеля:")
                    .Validate(n => n.Trim().Length >= 6
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Название отеля не может быть короче 6 символов![/]")));

            var managerIdInput = AnsiConsole.Prompt(
                new TextPrompt<string>("Вставьте ID нового менеджера (Guid) или нажмите Enter, чтобы оставить пустым:")
                    .AllowEmpty()
                    .Validate(input =>
                    {
                        if (string.IsNullOrWhiteSpace(input))
                            return ValidationResult.Success();

                        return Guid.TryParse(input, out _)
                            ? ValidationResult.Success()
                            : ValidationResult.Error("[red]Некорректный формат Guid менеджера! Введите валидный Id или оставьте поле пустым.[/]");
                    }));

            Guid? newManagerId = string.IsNullOrWhiteSpace(managerIdInput)
                ? null
                : Guid.Parse(managerIdInput);

            AnsiConsole.WriteLine();

            if (!AnsiConsole.Confirm("Вы уверены, что хотите сохранить изменения?"))
            {
                AnsiConsole.MarkupLine("[yellow]Обновление отменено пользователем[/]");
                await WaitForKeyPressAsync();
                return;
            }

            var command = new UpdateHotelCommand(hotelId, newName, newManagerId);

            await AnsiConsole.Status()
                .Spinner(Spinner.Known.Dots)
                .StartAsync("[yellow]Обновление...[/]", async ctx =>
                {
                    var result = await mediator.Send(command);

                    if (result.IsSuccess)
                    {
                        AnsiConsole.MarkupLine($"\n[green]Отель обновлен[/] ID: [bold]{result.Value}[/]");
                    }
                    else
                    {
                        AnsiConsole.MarkupLine($"\n[red]Ошибка при обновлении:[/] {result.ErrorMessage}");
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
