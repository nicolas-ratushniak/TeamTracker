namespace TeamTracker.Domain.Dto;

public class GameInfoCreateDto
{
    public Guid TeamHomeId { get; set; }
    public Guid TeamAwayId { get; set; }
    public int TeamHomeScore { get; set; }
    public int TeamAwayScore { get; set; }
}