using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleUI.Screens
{
    public abstract class BaseMenuScreen
    {
        protected abstract string GetHeader();
        protected abstract List<(string Title, Func<Task> Action)> ConfMenu();
        protected virtual string ExitOptionText => "Назад";
        protected virtual bool ShouldClose() => false;
        protected virtual Task OnExitAsync() => Task.CompletedTask;

        public virtual async Task ShowAsync()
        {
           
            bool isInside = true;

            while (isInside)
            {
                Console.Clear();
                AnsiConsole.Write(new Rule(GetHeader()));

                var selectionPromt = new SelectionPrompt<String>()
                    .AddChoices(ConfMenu().Select(i => i.Title));

                selectionPromt.AddChoice(ExitOptionText);
                var choice = AnsiConsole.Prompt(selectionPromt); 
                
                if(choice == ExitOptionText)
                {
                    isInside = false;
                    await OnExitAsync();
                    continue;
                }

                var selectedAction = ConfMenu()
                    .First(i => i.Title == choice).Action;
                    
                await selectedAction();

                if (ShouldClose())
                    isInside = false;
            }
        }
    }
}
