namespace TeamTracker.Wpf.ViewModels;

public class TeamDetailsItemViewModel
{
    public Guid Id { get; init; }
    public string Name { get; set; } = "";
    public string OriginCity { get; set; } = "";
    public int GamesWon { get; set; }
    public int GamesLost { get; set; }
    public int GamesDrawn { get; set; }
    public int MembersCount { get; set; }
    public int TotalGames { get; set; }
    public int Points { get; set; }
}