using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LibraryApp.Services;

namespace LibraryApp.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    private readonly AuthService _authService;
    private readonly ILibraryService _libraryService;

    [ObservableProperty] private string _username = string.Empty;
    [ObservableProperty] private string _password = string.Empty;
    [ObservableProperty] private string _errorMessage = string.Empty;

    public event System.Action<UserRole, object>? LoginSucceeded;

    public LoginViewModel(AuthService authService, ILibraryService libraryService)
    {
        _authService = authService;
        _libraryService = libraryService;
    }

    [RelayCommand]
    private void Login()
    {
        ErrorMessage = string.Empty;
        var (role, user) = _authService.Login(Username, Password);
        if (role == UserRole.None || user == null)
        {
            ErrorMessage = "Invalid username or password.";
            return;
        }
        LoginSucceeded?.Invoke(role, user);
    }
}
