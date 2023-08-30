namespace TeamTracker.Wpf.ViewModels;

public class TeamListItemViewModel
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public int Points { get; set; }
    public int TotalGames { get; set; }
    public int Wins { get; set; }
    public int Members { get; set; }
}