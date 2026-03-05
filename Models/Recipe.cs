namespace HomePlanner.Models;

public class Recipe
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<string> Ingredients { get; set; } = [];
    public string? Instructions { get; set; }
    public int? PrepTime { get; set; }
    public int? Servings { get; set; }
    public string? SourceUrl { get; set; }
    public int? CreatedById { get; set; }
    public User? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
