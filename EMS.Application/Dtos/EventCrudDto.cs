namespace EMS.Application.Dtos;

public class EventCrudDto
{
    public string? Name { get; set; }
    public DateTime Date { get; set; }
    public string? Location { get; set; }
    public string? Description { get; set; }
    public string? Image { get; set; }

    public string? Category { get; set; }
}
