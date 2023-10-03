using System;
using System.IO;
using System.Reflection;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using TeamTracker.Data;
using TeamTracker.Data.Models;
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
            .ConfigureAppConfiguration(builder => { builder.AddJsonFile("appsettings.json"); })
            .ConfigureServices((context, services) =>
            {
                services.AddSingleton<IModelConverter<Team>, ModelConverter<Team>>();
                services.AddSingleton<IModelConverter<GameInfo>, ModelConverter<GameInfo>>();

                var folder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var dbName = context.Configuration.GetRequiredSection("DbName").Value ?? "Db";

                var dbPath = Path.Combine(folder!, dbName);

                // ensure db folder exists
                Directory.CreateDirectory(dbPath);

                services.AddScoped<IRepository<Team>, Repository<Team>>(s =>
                {
                    var db = new TextFileDbTable(dbPath, "Teams");
                    return new Repository<Team>(db, s.GetRequiredService<IModelConverter<Team>>());
                });

                services.AddScoped<IRepository<GameInfo>, Repository<GameInfo>>(s =>
                {
                    var db = new TextFileDbTable(dbPath, "Games");
                    return new Repository<GameInfo>(db, s.GetRequiredService<IModelConverter<GameInfo>>());
                });

                services.AddScoped<ITeamService, TeamService>();
                services.AddScoped<IGameInfoService, GameInfoService>();
                services.AddScoped<IViewModelFactory, ViewModelFactory>();
                services.AddScoped<INavigator, Navigator>();

                services.AddSingleton<Func<TeamsViewModel>>(s => () => new TeamsViewModel(
                    s.GetRequiredService<ITeamService>(),
                    s.GetRequiredService<INavigator>(),
                    s.GetRequiredService<ILogger<TeamsViewModel>>()));

                services.AddSingleton<Func<TeamCreateViewModel>>(s => () => new TeamCreateViewModel(
                    s.GetRequiredService<ITeamService>(),
                    s.GetRequiredService<INavigator>(),
                    s.GetRequiredService<ILogger<TeamCreateViewModel>>()));

                services.AddSingleton<Func<Guid, TeamUpdateViewModel>>(s => id => new TeamUpdateViewModel(
                    id,
                    s.GetRequiredService<ITeamService>(),
                    s.GetRequiredService<INavigator>(),
                    s.GetRequiredService<ILogger<TeamUpdateViewModel>>()));

                services.AddSingleton<Func<GamesViewModel>>(s => () => new GamesViewModel(
                    s.GetRequiredService<IGameInfoService>(),
                    s.GetRequiredService<ITeamService>(),
                    s.GetRequiredService<INavigator>(),
                    s.GetRequiredService<ILogger<GamesViewModel>>()));

                services.AddSingleton<Func<GameCreateViewModel>>(s => () => new GameCreateViewModel(
                    s.GetRequiredService<IGameInfoService>(),
                    s.GetRequiredService<ITeamService>(),
                    s.GetRequiredService<INavigator>(),
                    s.GetRequiredService<ILogger<GameCreateViewModel>>()));

                services.AddSingleton<Func<HelpViewModel>>(_ => () => new HelpViewModel());

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