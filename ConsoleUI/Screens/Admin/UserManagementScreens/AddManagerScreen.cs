using Application.Common.Features.Users.Commands.ChangeUserRole;
using MediatR;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleUI.Screens.Admin.UserManagementScreens
{
    public class AddManagerScreen(IMediator mediator)
    {
        public async Task ShowAsync()
        {
            Console.Clear();
            AnsiConsole.Write(new Rule("[blue]НАЗНАЧИТЬ НОВОГО МЕНЕДЖЕРА[/]"));
            AnsiConsole.WriteLine();

            var username = AnsiConsole.Prompt(
                new TextPrompt<string>("Введите [yellow]Username[/] пользователя для назначения:")
                    .Validate(ctx => string.IsNullOrWhiteSpace(ctx)
                        ? ValidationResult.Error("[red]Имя пользователя не может быть пустым[/]")
                        : ValidationResult.Success()));

            AnsiConsole.WriteLine();
            if (!AnsiConsole.Confirm($"Вы уверены, что хотите дать пользователю [yellow]{username}[/] права [yellow]HotelManager[/]?"))
            {
                AnsiConsole.MarkupLine("\n[yellow]Операция отменена[/]");
                AnsiConsole.MarkupLine("[grey]Нажмите любую клавишу для возврата...[/]");
                Console.ReadKey(true);
                return;
            }

            bool isSuccess = false;
            string? errorMessage = null;

            await AnsiConsole.Status()
                .Spinner(Spinner.Known.Dots)
                .SpinnerStyle(Style.Parse("blue"))
                .StartAsync("Обновление прав пользователя в MongoDB...", async ctx =>
                {
                    var result = await mediator.Send(new ChangeUserRoleCommand(username, "HotelManager"));

                    if (result.IsSuccess)
                    {
                        isSuccess = true;
                    }
                    else
                    {
                        errorMessage = result.ErrorMessage;
                    }
                });

            AnsiConsole.WriteLine();
            if (isSuccess)
            {
                AnsiConsole.Write(new Panel(
                    $"Пользователь [yellow]{username}[/] назначен на роль [yellow]HotelManager[/]")
                    .BorderColor(Color.Green)
                    .RoundedBorder());
            }
            else
            {
                AnsiConsole.Write(new Panel($"[red]Ошибка обновления роли:[/] {errorMessage}")
                    .BorderColor(Color.Red)
                    .RoundedBorder());
            }

            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[grey]Нажмите любую клавишу, чтобы вернуться в меню управления...[/]");
            Console.ReadKey(true);
        }
    }
}