namespace TeamTracker.Domain.Models;

public class Team : ModelBase
{
    public string Name { get; set; } = "";
    public string OriginCity { get; set; } = "";
    public int GamesWon { get; set; }
    public int GamesLost { get; set; }
    public int GamesDrawn { get; set; }
    public int MembersCount { get; set; }
}