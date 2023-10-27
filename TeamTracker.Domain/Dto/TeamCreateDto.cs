using System.ComponentModel.DataAnnotations;

namespace TeamTracker.Domain.Dto;

public class TeamCreateDto
{
    [Required]
    [StringLength(20, MinimumLength = 2, ErrorMessage = "Name field length should be in range from 2 to 20")]
    [RegularExpression(@"^[A-Z]([\s\-]?[a-zA-Z\d])*$", ErrorMessage =
            "Name field can only contain letters, digits and \"-\". It should start with a capital letter and end with an alphanumeric")]
    public string Name { get; set; } = "";

    [Required]
    [StringLength(20, MinimumLength = 2, ErrorMessage = "Origin City field length should be in range from 2 to 20")]
    [RegularExpression(@"^[A-Z]([\s\-]?[a-zA-Z])*$", ErrorMessage =
            "Origin City can only contain letters and \"-\". It should start with a capital letter and end with a letter")]
    public string OriginCity { get; set; } = "";

    [Required]
    [Range(1, 900, ErrorMessage = "Members value should be in range from 1 to 900")]
    public int MembersCount { get; set; }
}