namespace EMS.Domain.Entities;

public class EventReview
{
    public int EventReviewId { get; set; }
    public int EventId { get; set; }
    public string UserId { get; set; }
    public int RatingStars { get; set; }
    public string ReviewText { get; set; }
    public DateTime CreatedAt { get; set; }
}
