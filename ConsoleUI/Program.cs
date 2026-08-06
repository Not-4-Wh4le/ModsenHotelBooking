using Application;
using ConsoleUI;
using ConsoleUI.Services;
using Infrastructure;
using Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Spectre.Console;

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.InputEncoding = System.Text.Encoding.UTF8;


var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, service) =>
    {
        service.AddApplication();
        service.AddInfrastructure();
        service.AddUi();
    })
    .Build();

using (var scope = host.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var seeder = services.GetRequiredService<DatabaseSeeder>();
        await seeder.SeedAsync();
         
    }
    catch (Exception ex)
    {
        AnsiConsole.MarkupLine($"[red]Ошибка: [/] {ex.Message}");
        AnsiConsole.WriteLine("Нажмите любую клавишу для выхода...");
        Console.ReadKey(true);
        return;
    }
}


using (var scope = host.Services.CreateScope())
{
    var engine = scope.ServiceProvider.GetRequiredService<ConsoleAppEngine>();
    await engine.RunAsync();
}