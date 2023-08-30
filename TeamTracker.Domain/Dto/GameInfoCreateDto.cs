using System.ComponentModel.DataAnnotations;

namespace TeamTracker.Domain.Dto;

public class GameInfoCreateDto
{
    [Required]
    public Guid TeamHomeId { get; set; }
    
    [Required]
    public Guid TeamAwayId { get; set; }
    
    [Required]
    [Range(0, 200)]
    public int TeamHomeScore { get; set; }
    
    [Required]
    [Range(0, 200)]
    public int TeamAwayScore { get; set; }

    [Required]
    public DateOnly Date { get; set; }
}