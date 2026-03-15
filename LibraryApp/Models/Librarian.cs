namespace LibraryApp.Models;

public class Librarian
{
    public string Id { get; set; } = System.Guid.NewGuid().ToString();
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}
