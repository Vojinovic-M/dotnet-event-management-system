﻿using System.ComponentModel.DataAnnotations;

namespace EMS.Domain.Entities
{
    public enum EventCategory
    {
        Conference,
        Seminar,
        Meeting,
        Workshop
    }

    public class Event
    {
        public int EventId { get; set; }
        public string? Name { get; set; }
        public DateTime Date { get; set; }
        public string? Location { get; set; }
        public EventCategory Category { get; set; }
        public  string? Description { get; set; }
        public string? ImageUrl { get; set; }
    }
}
