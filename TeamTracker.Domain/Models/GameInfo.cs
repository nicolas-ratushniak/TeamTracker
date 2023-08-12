namespace TeamTracker.Domain.Models;

public class GameInfo : Entity
{
    public Guid CupId { get; set; }
    public Guid TeamHomeId { get; set; }
    public Guid TeamAwayId { get; set; }
    public int TeamHomeScore { get; set; }
    public int TeamAwayScore { get; set; }
    public bool IsFinished { get; set; }
}