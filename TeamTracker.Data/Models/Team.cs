namespace TeamTracker.Data.Models;

public class Team : BaseModel
{
    public string Name { get; set; } = string.Empty;
    public string OriginCity { get; set; } = string.Empty;
    public int MembersCount { get; set; }
}