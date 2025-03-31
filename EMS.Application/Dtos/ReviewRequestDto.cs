namespace EMS.Application.Dtos;

public class ReviewRequestDto
{
    public int EventId { get; set; }
    public string UserId { get; set; }
    public int RatingStars { get; set; }
    public string ReviewText { get; set; }
}
