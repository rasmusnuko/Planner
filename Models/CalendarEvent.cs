namespace HomePlanner.Models;

public class CalendarEvent
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public string Type { get; set; } = "general"; // general | pickup | dinner
    public int? AssignedToId { get; set; }
    public User? AssignedTo { get; set; }
    public string? Notes { get; set; }
    public int? CreatedById { get; set; }
    public User? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
