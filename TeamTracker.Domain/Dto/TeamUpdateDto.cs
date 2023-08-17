namespace TeamTracker.Domain.Dto;

public class TeamUpdateDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public string OriginCity { get; set; } = "";
    public int MembersCount { get; set; }
}