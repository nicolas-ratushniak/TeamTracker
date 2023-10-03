namespace TeamTracker.Wpf.ViewModels.Inners;

public class GameDetailsItemViewModel
{
    public string Date { get; set; } = string.Empty;
    public string HomeTeamName { get; set; } = string.Empty;
    public string HomeTeamOriginCity { get; set; } = string.Empty;
    public int HomeTeamScore { get; set; }
    public string AwayTeamName { get; set; } = string.Empty;
    public string AwayTeamOriginCity { get; set; } = string.Empty;
    public int AwayTeamScore { get; set; }
}