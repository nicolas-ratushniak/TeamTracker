namespace TeamTracker.Domain.Models;

public class GameInfo : ModelBase
{
    public Guid TeamHomeId { get; set; }
    public Guid TeamAwayId { get; set; }
    public int TeamHomeScore { get; set; }
    public int TeamAwayScore { get; set; }
}