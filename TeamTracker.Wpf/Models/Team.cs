using TeamTracker.Wpf.Core;
using TeamTracker.Wpf.Data.Attributes;

namespace TeamTracker.Wpf.Models;

public class Team : BaseModel
{
    public string Name { get; set; } = "";
    public string OriginCity { get; set; } = "";
    public int TimesWon { get; set; }
    public int TimesLost { get; set; }
    public int TimesDraw { get; set; }
    public int MembersCount { get; set; }

    [NotTracked] 
    public int TotalGames => TimesWon + TimesLost + TimesDraw;
}