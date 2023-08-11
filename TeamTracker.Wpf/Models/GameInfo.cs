using TeamTracker.Wpf.Core;

namespace TeamTracker.Wpf.Models;

public class GameInfo : BaseModel
{
    public Guid CupId { get; set; }
    public Guid TeamHomeId { get; set; }
    public Guid TeamAwayId { get; set; }
    public int TeamHomeScore { get; set; }
    public int TeamAwayScore { get; set; }
    public bool IsFinished { get; set; }
}