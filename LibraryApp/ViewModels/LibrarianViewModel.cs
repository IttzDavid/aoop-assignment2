using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LibraryApp.Models;
using LibraryApp.Services;

namespace LibraryApp.ViewModels;

public partial class LibrarianViewModel : ViewModelBase
{
    private readonly Librarian _librarian;
    private readonly ILibraryService _libraryService;

    [ObservableProperty] private ObservableCollection<Book> _allBooks = new();
    [ObservableProperty] private Book? _selectedBook;
    [ObservableProperty] private string _catalogSearchText = string.Empty;
    [ObservableProperty] private string _catalogMessage = string.Empty;

    [ObservableProperty] private ObservableCollection<Loan> _activeLoans = new();
    [ObservableProperty] private int _totalActiveLoans;

    [ObservableProperty] private string _editTitle = string.Empty;
    [ObservableProperty] private string _editAuthor = string.Empty;
    [ObservableProperty] private string _editIsbn = string.Empty;
    [ObservableProperty] private string _editDescription = string.Empty;
    [ObservableProperty] private bool _isEditing;

    public string LibrarianName => _librarian.Name;

    public LibrarianViewModel(Librarian librarian, ILibraryService libraryService)
    {
        _librarian = librarian;
        _libraryService = libraryService;
        LoadCatalog();
        LoadActiveLoans();
    }

    partial void OnCatalogSearchTextChanged(string value) => LoadCatalog();

    partial void OnSelectedBookChanged(Book? value)
    {
        if (value != null)
        {
            EditTitle = value.Title;
            EditAuthor = value.Author;
            EditIsbn = value.ISBN;
            EditDescription = value.Description;
            IsEditing = true;
        }
        else
        {
            ClearForm();
        }
    }

    public void LoadCatalog()
    {
        var books = _libraryService.GetAllBooks()
            .Where(b => string.IsNullOrWhiteSpace(CatalogSearchText) ||
                        b.Title.Contains(CatalogSearchText, StringComparison.OrdinalIgnoreCase) ||
                        b.Author.Contains(CatalogSearchText, StringComparison.OrdinalIgnoreCase))
            .ToList();
        AllBooks = new ObservableCollection<Book>(books);
    }

    public void LoadActiveLoans()
    {
        var loans = _libraryService.GetActiveLoans();
        ActiveLoans = new ObservableCollection<Loan>(loans);
        TotalActiveLoans = loans.Count;
    }

    [RelayCommand]
    private void AddBook()
    {
        CatalogMessage = string.Empty;
        if (string.IsNullOrWhiteSpace(EditTitle)) { CatalogMessage = "Title is required."; return; }
        var book = new Book
        {
            Title = EditTitle.Trim(),
            Author = EditAuthor.Trim(),
            ISBN = EditIsbn.Trim(),
            Description = EditDescription.Trim(),
            IsAvailable = true
        };
        _libraryService.AddBook(book);
        _libraryService.Save();
        LoadCatalog();
        ClearForm();
        CatalogMessage = $"Book '{book.Title}' added successfully.";
    }

    [RelayCommand]
    private void SaveBook()
    {
        CatalogMessage = string.Empty;
        if (SelectedBook == null || string.IsNullOrWhiteSpace(EditTitle)) return;
        SelectedBook.Title = EditTitle.Trim();
        SelectedBook.Author = EditAuthor.Trim();
        SelectedBook.ISBN = EditIsbn.Trim();
        SelectedBook.Description = EditDescription.Trim();
        _libraryService.UpdateBook(SelectedBook);
        _libraryService.Save();
        LoadCatalog();
        CatalogMessage = $"Book '{SelectedBook.Title}' updated.";
    }

    [RelayCommand]
    private void DeleteBook()
    {
        CatalogMessage = string.Empty;
        if (SelectedBook == null) return;
        var title = SelectedBook.Title;
        _libraryService.DeleteBook(SelectedBook.Id);
        _libraryService.Save();
        ClearForm();
        LoadCatalog();
        LoadActiveLoans();
        CatalogMessage = $"Book '{title}' deleted.";
    }

    [RelayCommand]
    private void NewBook()
    {
        SelectedBook = null;
        ClearForm();
        IsEditing = false;
    }

    private void ClearForm()
    {
        EditTitle = string.Empty;
        EditAuthor = string.Empty;
        EditIsbn = string.Empty;
        EditDescription = string.Empty;
        IsEditing = false;
    }
}
