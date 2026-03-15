using System.Collections.Generic;
using LibraryApp.Models;

namespace LibraryApp.Services;

public interface ILibraryService
{
    List<Book> GetAllBooks();
    List<Book> GetAvailableBooks();
    List<Loan> GetActiveLoans();
    bool BorrowBook(string bookId, Member member);
    bool ReturnBook(string bookId, Member member);
    void AddBook(Book book);
    void UpdateBook(Book book);
    void DeleteBook(string bookId);
    void Save();
}
