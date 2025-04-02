using System.ComponentModel.DataAnnotations;

namespace EMS.Application.Dtos;

public class ReviewRequestDto
{
    [Required]
    public int EventId { get; set; }

    [Required]
    public string UserId { get; set; }

    [Range(1, 5)]
    public int RatingStars { get; set; }

    [StringLength(500, MinimumLength = 10)]
    public string ReviewText { get; set; }
}
