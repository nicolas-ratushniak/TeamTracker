using System.ComponentModel.DataAnnotations;

namespace TeamTracker.Domain.Dto;

public class GameInfoCreateDto
{
    [Required]
    public Guid TeamHomeId { get; set; }
    
    [Required]
    public Guid TeamAwayId { get; set; }
    
    [Required]
    [Range(0, int.MaxValue)]
    public int TeamHomeScore { get; set; }
    
    [Required]
    [Range(0, int.MaxValue)]
    public int TeamAwayScore { get; set; }
}