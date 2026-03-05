namespace HomePlanner.Models;

public class ShoppingItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = "other";
    public bool Checked { get; set; }
    public int? AddedById { get; set; }
    public User? AddedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
