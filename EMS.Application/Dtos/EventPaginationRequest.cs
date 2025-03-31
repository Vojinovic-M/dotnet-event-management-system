using System.ComponentModel.DataAnnotations;

namespace EMS.Application.Dtos;

public class EventPaginationRequest : PaginationRequest
{
    public DateTime? EventDate { get; set; }

    public string? Category { get; set; }

    [RegularExpression("date|name", ErrorMessage = "Invalid sort field")]
    public string SortBy { get; set; } = "date";

    [RegularExpression("asc|desc", ErrorMessage = "Invalid sort direction")]
    public string SortOrder { get; set; } = "asc";
    public bool UpcomingOnly { get; set; } = true;

}