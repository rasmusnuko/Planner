namespace HomePlanner.Models;

public class MealPlan
{
    public int Id { get; set; }
    public DateOnly Date { get; set; }
    public int? RecipeId { get; set; }
    public Recipe? Recipe { get; set; }
    public string? CustomMeal { get; set; }
    public string? Notes { get; set; }
}
