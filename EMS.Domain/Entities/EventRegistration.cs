namespace EMS.Domain.Entities;

public class EventRegistration
{
    public int EventId { get; set; }
    public Event? Event { get; set; }

    public string? UserId { get; set; }
    public ApplicationUser? User { get; set; }
}
