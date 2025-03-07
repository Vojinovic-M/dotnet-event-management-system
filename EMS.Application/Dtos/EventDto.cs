namespace EMS.Application.Dtos;

public class EventDto
{
    public int EventId { get; set; }
    public required string Name { get; set; }
    public DateTime Date { get; set; }
    public required string Location { get; set; }
    public required string Description { get; set; }
    public required string ImageUrl { get; set; }

}
