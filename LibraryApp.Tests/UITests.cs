using Avalonia.Headless.XUnit;
using LibraryApp.Models;
using LibraryApp.Services;
using LibraryApp.ViewModels;
using System.Collections.Generic;
using Xunit;

namespace LibraryApp.Tests;

public class UITests
{
    private static (AuthService authService, LibraryService libraryService) CreateServices()
    {
        var data = new AppData
        {
            Books = new List<Book>
            {
                new() { Id = "book1", Title = "Test Book", Author = "Test Author", IsAvailable = true },
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

        var dataService = new InMemoryDataService(data);
        var authService = new AuthService(dataService);
        var libraryService = new LibraryService(dataService, data);
        return (authService, libraryService);
    }

    private class InMemoryDataService : IDataService
    {
        private AppData _data;
        public InMemoryDataService(AppData data) { _data = data; }
        public AppData Load() => _data;
        public void Save(AppData data) { _data = data; }
    }

    [AvaloniaFact]
    public void UseCase1_MemberLogin_NavigatesToMemberView()
    {
        var (authService, libraryService) = CreateServices();
        var mainVm = new MainViewModel(authService, libraryService);

        mainVm.LoginViewModel.Username = "alice";
        mainVm.LoginViewModel.Password = "password123";
        mainVm.LoginViewModel.LoginCommand.Execute(null);

        Assert.IsType<MemberViewModel>(mainVm.CurrentViewModel);
    }

    [AvaloniaFact]
    public void UseCase2_MemberBorrowsBook_BookRemovedFromCatalog()
    {
        var (authService, libraryService) = CreateServices();
        var (role, user) = authService.Login("alice", "password123");
        var member = (Member)user!;
        var memberVm = new MemberViewModel(member, libraryService);

        Assert.Single(memberVm.CatalogBooks);
        memberVm.SelectedCatalogBook = memberVm.CatalogBooks[0];
        memberVm.BorrowBookCommand.Execute(null);

        Assert.Empty(memberVm.CatalogBooks);
        Assert.Single(memberVm.BorrowedBooks);
        Assert.Contains("Successfully borrowed", memberVm.CatalogMessage);
    }

    [AvaloniaFact]
    public void UseCase3_LibrarianViewsActiveLoans_AfterMemberBorrows()
    {
        var (authService, libraryService) = CreateServices();

        var (_, memberUser) = authService.Login("alice", "password123");
        var member = (Member)memberUser!;
        libraryService.BorrowBook("book1", member);

        var (libRole, libUser) = authService.Login("librarian", "libpass");
        var librarian = (Librarian)libUser!;
        var libVm = new LibrarianViewModel(librarian, libraryService);

        Assert.Equal(1, libVm.TotalActiveLoans);
        Assert.Single(libVm.ActiveLoans);
        Assert.Equal("Test Book", libVm.ActiveLoans[0].BookTitle);
    }
}
