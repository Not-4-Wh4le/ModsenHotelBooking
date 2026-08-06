using Application.Common.Features.Hotels.Commands.CreateHotel;
using MediatR;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleUI.Screens.Hotels
{
    public class CreateHotelScreen(IMediator mediator)
        : BaseLeafScreen
    {
        protected override async Task HandleAsync()
        {
            var name = AnsiConsole.Prompt(
                new TextPrompt<string>("Введите [blue]название[/] отеля:")
                    .Validate(n => n.Trim().Length >= 6
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Название отеля не может быть короче 6 символов[/]")));

            var country = AnsiConsole.Prompt(
                new TextPrompt<string>("Введите [blue]страну[/]:")
                    .Validate(c => !string.IsNullOrWhiteSpace(c)
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Страна не может быть пустой[/]")));

            var city = AnsiConsole.Prompt(
                new TextPrompt<string>("Введите [blue]город[/]:")
                    .Validate(c => !string.IsNullOrWhiteSpace(c)
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Город не может быть пустым[/]")));

            var address = AnsiConsole.Prompt(
                new TextPrompt<string>("Введите [blue]адрес[/]:")
                    .Validate(a => !string.IsNullOrWhiteSpace(a)
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Адрес не может быть пустым[/]")));

            var managerIdInput = AnsiConsole.Prompt(
                new TextPrompt<string>("Введите [blue]ID менеджера[/] (Guid) или нажмите Enter для пропуска:")
                    .AllowEmpty()
                    .Validate(input =>
                    {
                        if (string.IsNullOrWhiteSpace(input))
                            return ValidationResult.Success();

                        return Guid.TryParse(input, out _)
                            ? ValidationResult.Success()
                            : ValidationResult.Error("[red]Некорректный формат Guid");
                    }));

            Guid? managerId = string.IsNullOrWhiteSpace(managerIdInput)
                ? null
                : Guid.Parse(managerIdInput);

            AnsiConsole.WriteLine();

            if (!AnsiConsole.Confirm("Вы уверены, что хотите сохранить этот отель?"))
            {
                AnsiConsole.MarkupLine("[yellow]Операция отменена пользователем.[/]");
                return;
            }

            var command = new CreateHotelCommand(name, city, country, address, managerId);

            await AnsiConsole.Status()
                .Spinner(Spinner.Known.Dots)
                .StartAsync("[yellow]Сохранение отеля в базе данных...[/]", async ctx =>
                {
                    var result = await mediator.Send(command);

                    if (result.IsSuccess)
                    {
                        AnsiConsole.MarkupLine($"\n[green]Отель успешно создан![/] ID: [bold]{result.Value}[/]");
                    }
                    else
                    {
                        AnsiConsole.MarkupLine($"\n[red]Ошибка при создании отеля:[/] {result.ErrorMessage}");
                    }
                });
        }
        protected override string GetHeader() => "[green]ДОБАВЛЕНИЕ НОВОГО ОТЕЛЯ[/]";
    }
}
