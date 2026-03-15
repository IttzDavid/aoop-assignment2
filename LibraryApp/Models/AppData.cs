using System.Collections.Generic;

namespace LibraryApp.Models;

public class AppData
{
    public List<Book> Books { get; set; } = new();
    public List<Member> Members { get; set; } = new();
    public List<Librarian> Librarians { get; set; } = new();
    public List<Loan> ActiveLoans { get; set; } = new();
}
