using CommunityToolkit.Mvvm.ComponentModel;
using LibraryApp.Models;
using LibraryApp.Services;

namespace LibraryApp.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly AuthService _authService;
    private readonly ILibraryService _libraryService;

    [ObservableProperty] private ViewModelBase _currentViewModel;

    public LoginViewModel LoginViewModel { get; }

    public MainViewModel(AuthService authService, ILibraryService libraryService)
    {
        _authService = authService;
        _libraryService = libraryService;

        LoginViewModel = new LoginViewModel(authService, libraryService);
        LoginViewModel.LoginSucceeded += OnLoginSucceeded;
        _currentViewModel = LoginViewModel;
    }

    private void OnLoginSucceeded(UserRole role, object user)
    {
        if (role == UserRole.Member && user is Member member)
            CurrentViewModel = new MemberViewModel(member, _libraryService);
        else if (role == UserRole.Librarian && user is Librarian librarian)
            CurrentViewModel = new LibrarianViewModel(librarian, _libraryService);
    }
}
