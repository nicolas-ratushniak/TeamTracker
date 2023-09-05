namespace TeamTracker.Domain.Models;

public class Team : ModelBase
{
    public string Name { get; set; } = string.Empty;
    public string OriginCity { get; set; } = string.Empty;
    public int MembersCount { get; set; }
}