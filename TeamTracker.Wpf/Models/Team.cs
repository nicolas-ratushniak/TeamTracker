using TeamTracker.Wpf.Core;
using TeamTracker.Wpf.Data.Attributes;

namespace TeamTracker.Wpf.Models;

public class Team : BaseModel
{
    [Tracked] 
    public string Name { get; set; } = "";
    [Tracked] 
    public string OriginCity { get; set; } = "";
    [Tracked] 
    public int TimesWon { get; set; }
    [Tracked] 
    public int TimesLost { get; set; }
    [Tracked] 
    public int TimesDraw { get; set; }
    [Tracked] 
    public int MembersCount { get; set; }
    
    public int TotalGames => TimesWon + TimesLost + TimesDraw;
}