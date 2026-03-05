using System.Text.Json;
using System.Text.RegularExpressions;

namespace HomePlanner.Services;

public record ScrapedRecipe(
    string        Title,
    string?       Description,
    List<string>  Ingredients,
    string?       Instructions,
    int?          PrepTime,
    int?          Servings,
    string        SourceUrl);

public class RecipeScraperService(HttpClient http)
{
    private static readonly Regex JsonLdRegex = new(
        @"<script[^>]+type=""application/ld\+json""[^>]*>(.*?)</script>",
        RegexOptions.Singleline | RegexOptions.IgnoreCase);

    public async Task<ScrapedRecipe> ScrapeAsync(string url)
    {
        http.DefaultRequestHeaders.UserAgent.ParseAdd(
            "Mozilla/5.0 (compatible; HomePlanner/1.0)");

        string html;
        try
        {
            html = await http.GetStringAsync(url);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Could not fetch the URL. Check the address and try again. ({ex.Message})");
        }

        foreach (Match m in JsonLdRegex.Matches(html))
        {
            var json = m.Groups[1].Value.Trim();
            try
            {
                using var doc = JsonDocument.Parse(json);
                var recipeNode = FindRecipeNode(doc.RootElement);
                if (recipeNode is null) continue;
                return ParseRecipe(recipeNode.Value, url);
            }
            catch (JsonException) { /* skip malformed blocks */ }
        }

        throw new InvalidOperationException(
            "No recipe data found on this page. The site may not support Schema.org recipe markup.");
    }

    // Walk arrays / @graph to find a node with @type == "Recipe"
    private static JsonElement? FindRecipeNode(JsonElement root)
    {
        if (root.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in root.EnumerateArray())
            {
                var found = FindRecipeNode(item);
                if (found is not null) return found;
            }
            return null;
        }

        if (root.ValueKind != JsonValueKind.Object) return null;

        if (root.TryGetProperty("@graph", out var graph))
        {
            var found = FindRecipeNode(graph);
            if (found is not null) return found;
        }

        if (root.TryGetProperty("@type", out var typeProp))
        {
            var typeStr = typeProp.ValueKind == JsonValueKind.Array
                ? string.Join(' ', typeProp.EnumerateArray().Select(e => e.GetString() ?? ""))
                : typeProp.GetString() ?? "";

            if (typeStr.Contains("Recipe", StringComparison.OrdinalIgnoreCase))
                return root;
        }

        return null;
    }

    private static ScrapedRecipe ParseRecipe(JsonElement node, string sourceUrl)
    {
        var title = GetString(node, "name") ?? "Imported Recipe";

        var description = GetString(node, "description");

        var ingredients = new List<string>();
        if (node.TryGetProperty("recipeIngredient", out var ingArr)
            && ingArr.ValueKind == JsonValueKind.Array)
        {
            foreach (var ing in ingArr.EnumerateArray())
            {
                var s = ing.GetString();
                if (!string.IsNullOrWhiteSpace(s)) ingredients.Add(s.Trim());
            }
        }

        var instructions = ParseInstructions(node);

        var prepTime = ParseIsoDuration(GetString(node, "prepTime"))
                    ?? ParseIsoDuration(GetString(node, "totalTime"));

        int? servings = null;
        if (node.TryGetProperty("recipeYield", out var yieldEl))
        {
            var yieldStr = yieldEl.ValueKind == JsonValueKind.Array
                ? yieldEl.EnumerateArray().FirstOrDefault().GetString()
                : yieldEl.GetString();
            if (int.TryParse(ExtractLeadingNumber(yieldStr), out var y))
                servings = y;
        }

        return new ScrapedRecipe(title, description, ingredients, instructions, prepTime, servings, sourceUrl);
    }

    private static string? ParseInstructions(JsonElement node)
    {
        if (!node.TryGetProperty("recipeInstructions", out var inst)) return null;

        if (inst.ValueKind == JsonValueKind.String)
            return inst.GetString();

        if (inst.ValueKind == JsonValueKind.Array)
        {
            var steps = new List<string>();
            int stepNum = 1;
            foreach (var item in inst.EnumerateArray())
            {
                if (item.ValueKind == JsonValueKind.String)
                {
                    steps.Add($"{stepNum++}. {item.GetString()}");
                }
                else if (item.ValueKind == JsonValueKind.Object)
                {
                    var text = GetString(item, "text") ?? GetString(item, "name") ?? "";
                    if (!string.IsNullOrWhiteSpace(text))
                        steps.Add($"{stepNum++}. {text.Trim()}");
                }
            }
            return steps.Count > 0 ? string.Join("\n\n", steps) : null;
        }

        return null;
    }

    // ISO 8601 duration → minutes (e.g. "PT1H30M" → 90)
    private static int? ParseIsoDuration(string? iso)
    {
        if (string.IsNullOrWhiteSpace(iso)) return null;
        var match = Regex.Match(iso, @"PT(?:(\d+)H)?(?:(\d+)M)?", RegexOptions.IgnoreCase);
        if (!match.Success) return null;
        var hours   = match.Groups[1].Success ? int.Parse(match.Groups[1].Value) : 0;
        var minutes = match.Groups[2].Success ? int.Parse(match.Groups[2].Value) : 0;
        var total   = hours * 60 + minutes;
        return total > 0 ? total : null;
    }

    private static string? GetString(JsonElement el, string prop) =>
        el.TryGetProperty(prop, out var v) && v.ValueKind == JsonValueKind.String
            ? v.GetString()
            : null;

    private static string? ExtractLeadingNumber(string? s) =>
        s is null ? null : Regex.Match(s, @"\d+").Value is { Length: > 0 } m ? m : null;
}
