namespace TeamTracker.Domain.Models;

public class GameInfo : ModelBase
{
    public Guid SeasonId { get; set; }
    public Guid TeamHomeId { get; set; }
    public Guid TeamAwayId { get; set; }
    public int TeamHomeScore { get; set; }
    public int TeamAwayScore { get; set; }
}