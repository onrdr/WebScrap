namespace WebScrap;

public class Product2
{
    public int PageNumber { get; set; }
    public string? Brand { get; set; }
    public string? Name { get; set; }
    public string? ImageUrl { get; set; }
    public List<string?> Ingredients { get; set; } = [];
    public List<string?> QuickInfo { get; set; } = [];
}
