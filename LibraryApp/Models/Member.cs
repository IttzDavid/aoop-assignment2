using System.Collections.Generic;

namespace LibraryApp.Models;

public class Member
{
    public string Id { get; set; } = System.Guid.NewGuid().ToString();
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public List<string> BorrowedBookIds { get; set; } = new();
    public List<BorrowingHistoryEntry> BorrowingHistory { get; set; } = new();
}

public class BorrowingHistoryEntry
{
    public string BookId { get; set; } = string.Empty;
    public string BookTitle { get; set; } = string.Empty;
    public System.DateTime BorrowedDate { get; set; }
    public System.DateTime ReturnedDate { get; set; }
}
