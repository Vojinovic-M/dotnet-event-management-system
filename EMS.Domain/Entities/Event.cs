using System.ComponentModel.DataAnnotations;
using EMS.Domain.Enums;

namespace EMS.Domain.Entities
{

    public class Event
    {
        [Key]
        public int EventId { get; set; }
        public string? Name { get; set; }
        public DateTime Date { get; set; }
        public string? Location { get; set; }
        public EventCategory Category { get; set; }
        public  string? Description { get; set; }
        public string? Image { get; set; }
    }
}
