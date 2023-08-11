using TeamTracker.Wpf.Core;
using TeamTracker.Wpf.Data.Attributes;

namespace TeamTracker.Wpf.Models;

public class Team : BaseModel
{
    public string Name { get; set; } = "";
    public string OriginCity { get; set; } = "";
    public int GamesWon { get; set; }
    public int GamesLost { get; set; }
    public int GamesDrawn { get; set; }
    public int MembersCount { get; set; }

    [NotTracked] 
    public int TotalGames => GamesWon + GamesLost + GamesDrawn;
}