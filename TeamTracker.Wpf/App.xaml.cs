using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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

                services.AddSingleton<Func<TeamsViewModel>>(s => () => new TeamsViewModel(
                    s.GetRequiredService<ITeamService>(),
                    s.GetRequiredService<INavigator>()));
                
                services.AddSingleton<Func<TeamCreateFormViewModel>>(s => () => new TeamCreateFormViewModel(
                    s.GetRequiredService<ITeamService>(),
                    s.GetRequiredService<INavigator>(),
                    s.GetRequiredService<ILogger<TeamCreateFormViewModel>>()));
                
                services.AddSingleton<Func<Guid, TeamUpdateFormViewModel>>(s => id => new TeamUpdateFormViewModel(
                    id,
                    s.GetRequiredService<ITeamService>(),
                    s.GetRequiredService<INavigator>()));
                
                services.AddSingleton<Func<TeamsViewModel>>(s => () => new TeamsViewModel(
                    s.GetRequiredService<ITeamService>(),
                    s.GetRequiredService<INavigator>()));
                
                services.AddSingleton<Func<GamesViewModel>>(s => () => new GamesViewModel(
                    s.GetRequiredService<IGameInfoService>(),
                    s.GetRequiredService<ITeamService>(),
                    s.GetRequiredService<INavigator>()));

                services.AddSingleton<Func<GameCreateFormViewModel>>(s => () => new GameCreateFormViewModel(
                    s.GetRequiredService<IGameInfoService>(),
                    s.GetRequiredService<ITeamService>(),
                    s.GetRequiredService<INavigator>(),
                    s.GetRequiredService<ILogger<GameCreateFormViewModel>>()));

                services.AddSingleton<Func<HelpViewModel>>(s => () => new HelpViewModel());
                
                services.AddTransient<MainViewModel>();

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