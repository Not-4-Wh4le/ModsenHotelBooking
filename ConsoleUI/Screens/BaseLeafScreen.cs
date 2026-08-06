using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleUI.Screens
{
    public abstract class BaseLeafScreen
    {
        protected abstract string GetHeader();
        protected abstract Task HandleAsync();

        public async Task ShowAsync()
        {
            Console.Clear();
            AnsiConsole.Write(new Rule(GetHeader()));
            try
            {
                await HandleAsync();
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"\n[red]Критическая ошибка:[/] {ex.Message}");
            }
            await WaitForKeyPressAsync();
        }

        private static async Task WaitForKeyPressAsync()
        {
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[grey]Нажмите любую клавишу для возврата[/]");
            await Task.Run(() => Console.ReadKey(true));
        }
    }
}
