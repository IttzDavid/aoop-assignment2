using System.Collections.Generic;
using System.Linq;
using LibraryApp.Models;

namespace LibraryApp.Services;

public class LibraryService : ILibraryService
{
    private readonly IDataService _dataService;
    private AppData _data;

    public LibraryService(IDataService dataService, AppData data)
    {
        _dataService = dataService;
        _data = data;
    }

    public List<Book> GetAllBooks() => _data.Books;
    public List<Book> GetAvailableBooks() => _data.Books.Where(b => b.IsAvailable).ToList();
    public List<Loan> GetActiveLoans() => _data.ActiveLoans;

    public bool BorrowBook(string bookId, Member member)
    {
        var book = _data.Books.FirstOrDefault(b => b.Id == bookId);
        if (book == null || !book.IsAvailable) return false;

        book.IsAvailable = false;
        if (!member.BorrowedBookIds.Contains(bookId))
            member.BorrowedBookIds.Add(bookId);
        _data.ActiveLoans.Add(new Loan
        {
            BookId = bookId,
            BookTitle = book.Title,
            MemberId = member.Id,
            MemberName = member.Name,
            BorrowDate = System.DateTime.Now
        });
        return true;
    }

    public bool ReturnBook(string bookId, Member member)
    {
        var book = _data.Books.FirstOrDefault(b => b.Id == bookId);
        if (book == null) return false;

        book.IsAvailable = true;
        member.BorrowedBookIds.Remove(bookId);

        var loan = _data.ActiveLoans.FirstOrDefault(l => l.BookId == bookId && l.MemberId == member.Id);
        if (loan != null)
        {
            _data.ActiveLoans.Remove(loan);
            member.BorrowingHistory.Add(new BorrowingHistoryEntry
            {
                BookId = bookId,
                BookTitle = book.Title,
                BorrowedDate = loan.BorrowDate,
                ReturnedDate = System.DateTime.Now
            });
        }
        return true;
    }

    public void AddBook(Book book) => _data.Books.Add(book);

    public void UpdateBook(Book book)
    {
        var existing = _data.Books.FirstOrDefault(b => b.Id == book.Id);
        if (existing == null) return;
        existing.Title = book.Title;
        existing.Author = book.Author;
        existing.ISBN = book.ISBN;
        existing.Description = book.Description;
        existing.IsAvailable = book.IsAvailable;
        existing.Ratings = book.Ratings;
    }

    public void DeleteBook(string bookId)
    {
        _data.Books.RemoveAll(b => b.Id == bookId);
        _data.ActiveLoans.RemoveAll(l => l.BookId == bookId);
        foreach (var member in _data.Members)
            member.BorrowedBookIds.Remove(bookId);
    }

    public void Save() => _dataService.Save(_data);
}
