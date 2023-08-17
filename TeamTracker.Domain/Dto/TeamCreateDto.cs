namespace TeamTracker.Domain.Dto;

public class TeamCreateDto
{
    public string Name { get; set; } = "";
    public string OriginCity { get; set; } = "";
    public int MembersCount { get; set; }
}