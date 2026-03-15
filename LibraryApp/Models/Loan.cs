namespace LibraryApp.Models;

public class Loan
{
    public string Id { get; set; } = System.Guid.NewGuid().ToString();
    public string BookId { get; set; } = string.Empty;
    public string BookTitle { get; set; } = string.Empty;
    public string MemberId { get; set; } = string.Empty;
    public string MemberName { get; set; } = string.Empty;
    public System.DateTime BorrowDate { get; set; } = System.DateTime.Now;
}
