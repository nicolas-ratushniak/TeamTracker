using System;

namespace TeamTracker.Wpf.ViewModels.Inners;

public class GameListItemViewModel
{
    public Guid Id { get; set; }
    public DateOnly Date { get; set; }
    public string HomeTeamName { get; set; } = string.Empty;
    public string HomeTeamOriginCity { get; set; } = string.Empty;
    public int HomeTeamScore { get; set; }
    public string AwayTeamName { get; set; } = string.Empty;
    public string AwayTeamOriginCity { get; set; } = string.Empty;
    public int AwayTeamScore { get; set; }
    public int GoalDifference => Math.Abs(HomeTeamScore - AwayTeamScore);
    public string HomeTeamFullName => $"{HomeTeamName}-{HomeTeamOriginCity}";
    public string AwayTeamFullName => $"{AwayTeamName}-{AwayTeamOriginCity}";
}