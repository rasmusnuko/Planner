namespace HomePlanner.Models;

public class TodoItem
{
    public int Id { get; set; }
    public int ListId { get; set; }
    public TodoList? List { get; set; }
    public string Text { get; set; } = string.Empty;
    public bool Done { get; set; }
    public int? AssignedToId { get; set; }
    public User? AssignedTo { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
