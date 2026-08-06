using Application.Common.Features.Users.Commands.LoginUser;
using ConsoleUI.Services;
using MediatR;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleUI.Screens
{
    public class LoginScreen(IMediator mediator, CurrentUserService currentUser)
        : BaseLeafScreen
    {
        protected override string GetHeader() => "[blue]ВХОД В СИСТЕМУ[/]";

        protected override async Task HandleAsync()
        {
            var username = AnsiConsole.Prompt(
                 new TextPrompt<string>("Введите [green]Username[/]:")
                     .Validate(ctx => string.IsNullOrWhiteSpace(ctx)
                         ? ValidationResult.Error("[red]Имя пользователя не может быть пустым[/]")
                         : ValidationResult.Success()));

            var password = AnsiConsole.Prompt(
                new TextPrompt<string>("Введите [green]Пароль[/]:")
                    .Secret('*')
                    .Validate(ctx => string.IsNullOrWhiteSpace(ctx)
                        ? ValidationResult.Error("[red]Пароль не может быть пустым[/]")
                        : ValidationResult.Success()));

            UserSessionDto? sessionDto = null;
            string? errorMessage = null;

            await AnsiConsole.Status()
                .Spinner(Spinner.Known.Dots)
                .SpinnerStyle(Style.Parse("cyan"))
                .StartAsync("Проверка учетных данных...", async ctx =>
                {
                    var result = await mediator.Send(new LoginUserCommand(username, password));
                    if (result.IsSuccess)
                    {
                        sessionDto = result.Value;
                    }
                    else
                    {
                        errorMessage = result.ErrorMessage;
                    }
                });

            if (sessionDto != null)
            {
                currentUser.Authenticate(sessionDto.Id, sessionDto.Username, sessionDto.Email, sessionDto.Role);
                AnsiConsole.MarkupLine($"\n[green]Добро пожаловать, {sessionDto.Username}![/]");
            }
            else
            {
                AnsiConsole.Write(new Panel($"[red]Ошибка входа:[/] {errorMessage}").BorderColor(Color.Red).RoundedBorder());
            }
        }
    }
}
