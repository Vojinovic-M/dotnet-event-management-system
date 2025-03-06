namespace EMS.Domain.Entities;

public class Event
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public DateTime Date { get; set; }
    public required string Location { get; set; }
    public enum Category
    {
        Conference,
        Seminar,
        Meeting,
        Workshop
    }
    public required string Description { get; set; }
    public required string ImageUrl { get; set; }
}
