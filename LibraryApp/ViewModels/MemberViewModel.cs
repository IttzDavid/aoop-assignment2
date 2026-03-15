using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LibraryApp.Models;
using LibraryApp.Services;

namespace LibraryApp.ViewModels;

public partial class MemberViewModel : ViewModelBase
{
    private readonly Member _member;
    private readonly ILibraryService _libraryService;

    [ObservableProperty] private ObservableCollection<Book> _catalogBooks = new();
    [ObservableProperty] private Book? _selectedCatalogBook;
    [ObservableProperty] private string _catalogSearchText = string.Empty;
    [ObservableProperty] private string _catalogMessage = string.Empty;

    [ObservableProperty] private ObservableCollection<Book> _borrowedBooks = new();
    [ObservableProperty] private Book? _selectedBorrowedBook;
    [ObservableProperty] private string _loansMessage = string.Empty;

    public string MemberName => _member.Name;

    public MemberViewModel(Member member, ILibraryService libraryService)
    {
        _member = member;
        _libraryService = libraryService;
        LoadCatalog();
        LoadBorrowedBooks();
    }

    partial void OnCatalogSearchTextChanged(string value) => LoadCatalog();

    public void LoadCatalog()
    {
        var books = _libraryService.GetAvailableBooks()
            .Where(b => string.IsNullOrWhiteSpace(CatalogSearchText) ||
                        b.Title.Contains(CatalogSearchText, StringComparison.OrdinalIgnoreCase) ||
                        b.Author.Contains(CatalogSearchText, StringComparison.OrdinalIgnoreCase))
            .ToList();
        CatalogBooks = new ObservableCollection<Book>(books);
    }

    public void LoadBorrowedBooks()
    {
        var borrowed = _libraryService.GetAllBooks()
            .Where(b => _member.BorrowedBookIds.Contains(b.Id))
            .ToList();
        BorrowedBooks = new ObservableCollection<Book>(borrowed);
    }

    [RelayCommand]
    private void BorrowBook()
    {
        CatalogMessage = string.Empty;
        if (SelectedCatalogBook == null)
        {
            CatalogMessage = "Please select a book to borrow.";
            return;
        }
        var success = _libraryService.BorrowBook(SelectedCatalogBook.Id, _member);
        if (success)
        {
            CatalogMessage = $"Successfully borrowed '{SelectedCatalogBook.Title}'!";
            _libraryService.Save();
            LoadCatalog();
            LoadBorrowedBooks();
        }
        else
        {
            CatalogMessage = "Book is no longer available.";
        }
    }

    [RelayCommand]
    private void ReturnBook()
    {
        LoansMessage = string.Empty;
        if (SelectedBorrowedBook == null)
        {
            LoansMessage = "Please select a book to return.";
            return;
        }
        var bookTitle = SelectedBorrowedBook.Title;
        var success = _libraryService.ReturnBook(SelectedBorrowedBook.Id, _member);
        if (success)
        {
            LoansMessage = $"Successfully returned '{bookTitle}'!";
            _libraryService.Save();
            LoadCatalog();
            LoadBorrowedBooks();
        }
        else
        {
            LoansMessage = "Could not return book.";
        }
    }
}
