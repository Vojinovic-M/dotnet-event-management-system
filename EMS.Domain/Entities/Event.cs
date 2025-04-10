﻿using System.ComponentModel.DataAnnotations;
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
        public double AverageRating { get; set; }
        public int ReviewsCount { get; set; }

        public ICollection<EventOwner>? EventOwners { get; set; }
        public ICollection<EventRegistration>? EventRegistrations { get; set; }
        public ICollection<EventReview> EventReviews { get; set; }
    }
}
