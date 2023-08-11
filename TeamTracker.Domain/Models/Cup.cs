namespace TeamTracker.Domain.Models;

public class Cup : BaseModel
{
    public string Name { get; set; } = "";
    public Guid? WinnerTeam { get; set; }
    public bool IsFinished { get; set; }
}