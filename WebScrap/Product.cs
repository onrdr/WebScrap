namespace WebScrap;

public class Product
{
    public int PageNumber { get; set; }
    public string? Name { get; set; }
    public string? Explained { get; set; }
    public List<string> WhatItIs { get; set; } = [];
    public List<string> Benefits { get; set; } = [];
    public List<string> Concerns { get; set; } = [];
    public List<string> WhatItDoes { get; set; } = [];
    public List<string> AlternativeNames { get; set; } = [];
    public CosIngData CosIngData { get; set; }
}

public class CosIngData
{
    public string? CosIngID { get; set; }
    public string? INCIName { get; set; }
    public string? InnName { get; set; }
    public string? ECNumber { get; set; }
    public string? PhEurName { get; set; }
    public string? AllFunctions { get; set; }
}
