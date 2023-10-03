namespace TeamTracker.Data.Models;

public class GameInfo : BaseModel
{
    public Guid TeamHomeId { get; set; }
    public Guid TeamAwayId { get; set; }
    public int TeamHomeScore { get; set; }
    public int TeamAwayScore { get; set; }
    public DateOnly Date { get; set; }
}