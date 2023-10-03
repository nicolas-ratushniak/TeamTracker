namespace TeamTracker.Wpf.ViewModels.Inners;

public class TeamDetailsItemViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string OriginCity { get; set; } = string.Empty;
    public int GamesWon { get; set; }
    public int GamesLost { get; set; }
    public int GamesDrawn { get; set; }
    public int MembersCount { get; set; }
    public int TotalGames { get; set; }
    public int Points { get; set; }
}