using TeamTracker.Wpf.Core;

namespace TeamTracker.Wpf.Models;

public class Cup : BaseModel
{
    public string Name { get; set; } = "";
    public Guid? WinnerTeam { get; set; }
    public bool IsFinished { get; set; }
}