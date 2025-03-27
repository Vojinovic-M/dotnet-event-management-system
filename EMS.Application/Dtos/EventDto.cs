namespace EMS.Application.Dtos;

public class EventDto
{
    public int EventId { get; set; }
    public string? Name { get; set; }
    public DateTime Date { get; set; }
    public string? Location { get; set; }
    public string? Description { get; set; }
    public string? Image { get; set; }

    public string? Category { get; set; }
    public string? UserId { get; set; }
}
