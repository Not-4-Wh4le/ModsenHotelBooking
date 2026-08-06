using Application.Common.Features.Users.Commands.RegisterUser;
using MediatR;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleUI.Screens
{
    public class RegisterScreen(IMediator mediator)
    {
        public async Task ShowAsync()
        {
            Console.Clear();
            AnsiConsole.Write(new Rule("[blue]РЕГИСТРАЦИЯ НОВОГО ПОЛЬЗОВАТЕЛЯ[/]"));

            var username = AnsiConsole.Prompt(
                new TextPrompt<string>("Придумайте [green]Username[/]:")
                    .Validate(ctx => string.IsNullOrWhiteSpace(ctx) || ctx.Length < 6
                        ? ValidationResult.Error("[red]Некорректное имя пользователя[/]")
                        : ValidationResult.Success() 
                    ));

            var email = AnsiConsole.Prompt(
                new TextPrompt<string>("Введите ваш [green]Email[/]:")
                    .Validate(ctx => !ctx.Contains("@") || !ctx.Contains(".")
                        ? ValidationResult.Error("[red]Некорректный формат Email[/]")
                        : ValidationResult.Success()));

            var password = AnsiConsole.Prompt(
                new TextPrompt<string>("Придумайте [green]Пароль[/]:")
                    .Secret('*')
                    .Validate(ctx => ctx.Length < 6
                        ? ValidationResult.Error("[red]Пароль должен быть не менее 6 символов[/]")
                        : ValidationResult.Success()));

            bool isSuccess = false;
            string? errorMessage = null;

            await AnsiConsole.Status()
                .Spinner(Spinner.Known.Dots)
                .SpinnerStyle(Style.Parse("green"))
                .StartAsync("Подождите...", async ctx =>
                {
                    var result = await mediator.Send(new RegisterUserCommand(username, email, password));
                    if (result.IsSuccess) isSuccess = true;
                    else errorMessage = result.ErrorMessage;
                });

            if (isSuccess)
            {
                AnsiConsole.Write(new Panel($"[green]Пользователь {username} успешно зарегистрирован[/]\n")
                    .BorderColor(Color.Green).Header("[bold white] Успех [/]").RoundedBorder());
            }
            else
            {
                AnsiConsole.Write(new Panel($"[red]Ошибка регистрации:[/] {errorMessage}").BorderColor(Color.Red).RoundedBorder());
            }

            AnsiConsole.MarkupLine("\n[grey]Нажмите любую клавишу, чтобы вернуться в меню...[/]");
            Console.ReadKey(true);
        }
    }
}
