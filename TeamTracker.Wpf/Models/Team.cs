using TeamTracker.Wpf.Core;

namespace TeamTracker.Wpf.Models;

public class Team : BaseModel
{
    public string Name { get; set; } = "";
    public string OriginCity { get; set; } = "";
    public int GamesWon { get; set; }
    public int GamesLost { get; set; }
    public int GamesDrawn { get; set; }
    public int MembersCount { get; set; }
}