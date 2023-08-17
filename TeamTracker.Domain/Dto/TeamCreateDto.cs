using System.ComponentModel.DataAnnotations;

namespace TeamTracker.Domain.Dto;

public class TeamCreateDto
{
    [Required]
    [StringLength(30, MinimumLength = 2)]
    public string Name { get; set; } = "";
    
    [Required]
    [StringLength(40, MinimumLength = 2)]
    public string OriginCity { get; set; } = "";
    
    [Required]
    [Range(0, int.MaxValue)]
    public int MembersCount { get; set; }
}