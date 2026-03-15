using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace LibraryApp.Models;

public class Book
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string ISBN { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsAvailable { get; set; } = true;
    public List<int> Ratings { get; set; } = new();

    [JsonIgnore]
    public double AverageRating => Ratings.Count == 0 ? 0 : (double)Ratings.Sum() / Ratings.Count;
}
