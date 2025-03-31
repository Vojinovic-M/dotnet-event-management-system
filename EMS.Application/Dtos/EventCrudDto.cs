using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace EMS.Application.Dtos;

public class EventCrudDto
{
    [Required]
    public string? Name { get; set; }
    [Required]
    public DateTime Date { get; set; }
    [Required]
    public string? Location { get; set; }
    [Required]
    public string? Description { get; set; }
    [Required]
    public IFormFile? Image { get; set; }
    [Required]
    public string? Category { get; set; }
}
