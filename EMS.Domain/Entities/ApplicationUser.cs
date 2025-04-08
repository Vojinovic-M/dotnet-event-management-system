using Microsoft.AspNetCore.Identity;

namespace EMS.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public ICollection<EventOwner>? EventOwners { get; set; }
    public ICollection<EventRegistration>? EventRegistrations { get; set; }
}
