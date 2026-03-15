using LibraryApp.Models;

namespace LibraryApp.Services;

public enum UserRole { None, Member, Librarian }

public interface IAuthService
{
    (UserRole Role, object? User) Login(string username, string password);
}
