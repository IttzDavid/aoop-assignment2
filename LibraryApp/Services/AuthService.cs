using LibraryApp.Models;

namespace LibraryApp.Services;

public class AuthService : IAuthService
{
    private readonly IDataService _dataService;
    private AppData _data;

    public AuthService(IDataService dataService)
    {
        _dataService = dataService;
        _data = dataService.Load();
    }

    public AppData Data => _data;

    public (UserRole Role, object? User) Login(string username, string password)
    {
        _data = _dataService.Load();
        var member = _data.Members.Find(m => m.Username == username && PasswordHelper.Verify(password, m.Password));
        if (member != null) return (UserRole.Member, member);

        var librarian = _data.Librarians.Find(l => l.Username == username && PasswordHelper.Verify(password, l.Password));
        if (librarian != null) return (UserRole.Librarian, librarian);

        return (UserRole.None, null);
    }
}
