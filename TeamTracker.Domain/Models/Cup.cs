namespace TeamTracker.Domain.Models;

public class Cup
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public Guid? WinnerTeam { get; set; }
    public bool IsFinished { get; set; }
}