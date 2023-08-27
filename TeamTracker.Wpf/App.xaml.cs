using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using TeamTracker.Data;
using TeamTracker.Domain.Data;
using TeamTracker.Domain.Models;
using TeamTracker.Domain.Services;
using TeamTracker.Wpf.Navigation;
using TeamTracker.Wpf.ViewModels;
using TeamTracker.Wpf.ViewModels.Factories;

namespace TeamTracker.Wpf;

public partial class App : Application
{
    private static readonly IHost AppHost;

    static App()
    {
        AppHost = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration(builder =>
            {
                builder.AddJsonFile("appsettings.json");
            })
            .ConfigureServices((context, services) =>
            {
                services.AddSingleton<IModelConverter<Team>, ModelConverter<Team>>();
                services.AddSingleton<IModelConverter<GameInfo>, ModelConverter<GameInfo>>();

                var dbPath = context.Configuration.GetRequiredSection("TeamTracker:DesignTimeDB").Value;
                
                services.AddScoped<IRepository<Team>, Repository<Team>>(s =>
                {
                    var db = new TextFileDbTable(dbPath!, "TeamsTable");
                    var converter = s.GetRequiredService<IModelConverter<Team>>();
                    return new Repository<Team>(db, converter);
                });

                services.AddScoped<IRepository<GameInfo>, Repository<GameInfo>>(s =>
                {
                    var db = new TextFileDbTable(dbPath!, "GamesTable");
                    var converter = s.GetRequiredService<IModelConverter<GameInfo>>();
                    return new Repository<GameInfo>(db, converter);
                });

                services.AddScoped<ITeamService, TeamService>();
                services.AddScoped<IGameInfoService, GameInfoService>();
                services.AddScoped<IViewModelFactory, ViewModelFactory>();
                services.AddScoped<INavigator, Navigator>();

                services.AddTransient<TeamsViewModel>();
                services.AddTransient<GamesViewModel>();
                services.AddTransient<HelpViewModel>();
                services.AddTransient<MainViewModel>();
                services.AddTransient<GamesViewModel>();

                services.AddScoped<MainWindow>(s =>
                    new MainWindow(s.GetRequiredService<MainViewModel>()));
            })
            .UseSerilog((context, loggerConfiguration) =>
            {
                loggerConfiguration.ReadFrom.Configuration(context.Configuration);
            })
            .Build();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        await AppHost.StartAsync();
        MainWindow = AppHost.Services.GetRequiredService<MainWindow>();
        MainWindow.Show();
        
        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await AppHost.StopAsync();
        
        base.OnExit(e);
    }
}