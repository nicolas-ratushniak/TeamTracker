using System.ComponentModel.DataAnnotations;

namespace TeamTracker.Domain.Dto;

public class GameInfoCreateDto
{
    [Required] public Guid TeamHomeId { get; set; }

    [Required] public Guid TeamAwayId { get; set; }

    [Required]
    [Range(0, 99, ErrorMessage = "Home Team Score value should be in range from 0 to 99")]
    public int TeamHomeScore { get; set; }

    [Required]
    [Range(0, 99, ErrorMessage = "Away Team Score value should be in range from 0 to 99")]
    public int TeamAwayScore { get; set; }

    [Required] public DateOnly Date { get; set; }
}