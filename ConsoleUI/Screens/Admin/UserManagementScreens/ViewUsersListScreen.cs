using Application.Common.Features.Users.Queries.GetUsersList;
using MediatR;
using Spectre.Console;


namespace ModsenHotelBooking.ConsoleUI.Screens.Admin;

public class ViewUsersListScreen(IMediator mediator)
{
    public async Task ShowAsync()
    {
        int currentPage = 1;
        const int pageSize = 5; 
        bool isInside = true;

        while (isInside)
        {
            Console.Clear();
            AnsiConsole.Write(new Rule($"[blue]УПРАВЛЕНИЕ ПОЛЬЗОВАТЕЛЯМИ[/]"));
            AnsiConsole.WriteLine();

            var query = new GetUsersListQuery(Page: currentPage, PageSize: pageSize);

            bool isSuccess = false;
            string? errorMessage = null;
            IReadOnlyList<UserDto> users = Array.Empty<UserDto>();
            int totalCount = 0;

            await AnsiConsole.Status()
                .Spinner(Spinner.Known.Dots)
                .SpinnerStyle(Style.Parse("blue"))
                .StartAsync($"Загрузка страницы {currentPage}...", async ctx =>
                {
                    var result = await mediator.Send(query);
                    if (result.IsSuccess)
                    {
                        isSuccess = true;
                        users = result.Value!.Items;      
                        totalCount = result.Value.TotalCount; 
                    }
                    else
                    {
                        errorMessage = result.ErrorMessage;
                    }
                });

            if (!isSuccess)
            {
                AnsiConsole.Write(new Panel($"[red]Ошибка:[/] {errorMessage}").BorderColor(Color.Red));
                AnsiConsole.MarkupLine("\n[grey]Нажмите любую клавишу для возврата...[/]");
                Console.ReadKey(true);
                return;
            }


            int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            if (totalPages == 0) totalPages = 1;

            if (users.Count == 0 && currentPage == 1)
            {
                AnsiConsole.MarkupLine("[yellow]В базе данных пока нет пользователей.[/]\n");
            }
            else
            {
                var table = new Table().Border(TableBorder.Rounded).BorderColor(Color.Blue);
                table.Title($"[bold white]Страница {currentPage} из {totalPages}[/]");

                table.AddColumn(new TableColumn("[yellow]ID[/]").Centered());
                table.AddColumn(new TableColumn("[yellow]Username[/]"));
                table.AddColumn(new TableColumn("[yellow]Email[/]"));
                table.AddColumn(new TableColumn("[yellow]Роль[/]").Centered());
                table.AddColumn(new TableColumn("[yellow]Статус[/]").Centered());

                foreach (var user in users)
                {
                    var status = user.IsDeleted ? "[red]Удален[/]" : "[green]Активен[/]";
                    table.AddRow(user.Id.ToString(), user.Username, user.Email, $"[bold]{user.Role}[/]", status);
                }

                AnsiConsole.Write(table);
                AnsiConsole.MarkupLine($"Всего пользователей в системе: {totalCount}\n");
            }

           
            var prompt = new SelectionPrompt<string>();

            const string nextChoice = "-> Следующая страница";
            const string prevChoice = "<- Предыдущая страница";
            const string exitChoice = "Вернуться в меню управления";

            if (currentPage < totalPages)
            {
                prompt.AddChoice(nextChoice);
            }

            if (currentPage > 1)
            {
                prompt.AddChoice(prevChoice);
            }

            prompt.AddChoice(exitChoice);

            var choice = AnsiConsole.Prompt(prompt);

            switch (choice)
            {
                case nextChoice:
                    currentPage++; 
                    break;

                case prevChoice:
                    currentPage--; 
                    break;

                case exitChoice:
                    isInside = false; 
                    break;
            }
        }
    }
}