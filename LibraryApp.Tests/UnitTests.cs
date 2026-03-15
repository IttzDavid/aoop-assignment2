using System.Collections.Generic;
using LibraryApp.Models;
using LibraryApp.Services;
using Xunit;

namespace LibraryApp.Tests;

public class UnitTests
{
    private static AppData CreateTestData() => new AppData
    {
        Books = new List<Book>
        {
            new() { Id = "book1", Title = "Test Book 1", Author = "Author 1", IsAvailable = true },
            new() { Id = "book2", Title = "Test Book 2", Author = "Author 2", IsAvailable = false },
        },
        Members = new List<Member>
        {
            new() { Id = "member1", Username = "alice", Password = PasswordHelper.Hash("password123"), Name = "Alice Smith" },
        },
        Librarians = new List<Librarian>
        {
            new() { Id = "lib1", Username = "librarian", Password = PasswordHelper.Hash("libpass"), Name = "Carol Williams" },
        },
        ActiveLoans = new List<Loan>()
    };

    private class InMemoryDataService : IDataService
    {
        private AppData _data;
        public InMemoryDataService(AppData data) { _data = data; }
        public AppData Load() => _data;
        public void Save(AppData data) { _data = data; }
    }

    [Fact]
    public void BorrowBook_Success_BookBecomesUnavailable()
    {
        var data = CreateTestData();
        var service = new LibraryService(new InMemoryDataService(data), data);
        var member = data.Members[0];

        var result = service.BorrowBook("book1", member);

        Assert.True(result);
        Assert.False(data.Books[0].IsAvailable);
        Assert.Contains("book1", member.BorrowedBookIds);
        Assert.Single(data.ActiveLoans);
    }

    [Fact]
    public void ReturnBook_Success_BookBecomesAvailable()
    {
        var data = CreateTestData();
        var service = new LibraryService(new InMemoryDataService(data), data);
        var member = data.Members[0];
        service.BorrowBook("book1", member);

        var result = service.ReturnBook("book1", member);

        Assert.True(result);
        Assert.True(data.Books[0].IsAvailable);
        Assert.DoesNotContain("book1", member.BorrowedBookIds);
        Assert.Empty(data.ActiveLoans);
        Assert.Single(member.BorrowingHistory);
    }

    [Fact]
    public void BorrowBook_UnavailableBook_ReturnsFalse()
    {
        var data = CreateTestData();
        var service = new LibraryService(new InMemoryDataService(data), data);
        var member = data.Members[0];

        var result = service.BorrowBook("book2", member);

        Assert.False(result);
    }

    [Fact]
    public void Login_ValidMember_ReturnsMemberRole()
    {
        var data = CreateTestData();
        var dataService = new InMemoryDataService(data);
        var authService = new AuthService(dataService);

        var (role, user) = authService.Login("alice", "password123");

        Assert.Equal(UserRole.Member, role);
        Assert.NotNull(user);
        Assert.IsType<Member>(user);
    }

    [Fact]
    public void Login_InvalidCredentials_ReturnsNone()
    {
        var data = CreateTestData();
        var dataService = new InMemoryDataService(data);
        var authService = new AuthService(dataService);

        var (role, user) = authService.Login("alice", "wrongpassword");

        Assert.Equal(UserRole.None, role);
        Assert.Null(user);
    }

    [Fact]
    public void AddBook_AddsToList()
    {
        var data = CreateTestData();
        var service = new LibraryService(new InMemoryDataService(data), data);
        var newBook = new Book { Title = "New Book", Author = "New Author" };

        service.AddBook(newBook);

        Assert.Equal(3, data.Books.Count);
        Assert.Contains(data.Books, b => b.Title == "New Book");
    }

    [Fact]
    public void DeleteBook_RemovesFromList()
    {
        var data = CreateTestData();
        var service = new LibraryService(new InMemoryDataService(data), data);

        service.DeleteBook("book1");

        Assert.Single(data.Books);
        Assert.DoesNotContain(data.Books, b => b.Id == "book1");
    }

    [Fact]
    public void DeleteBook_CleansUpMemberBorrowedIds()
    {
        var data = CreateTestData();
        var service = new LibraryService(new InMemoryDataService(data), data);
        var member = data.Members[0];
        service.BorrowBook("book1", member);
        Assert.Contains("book1", member.BorrowedBookIds);

        service.DeleteBook("book1");

        Assert.DoesNotContain("book1", member.BorrowedBookIds);
        Assert.Empty(data.ActiveLoans);
    }
}
