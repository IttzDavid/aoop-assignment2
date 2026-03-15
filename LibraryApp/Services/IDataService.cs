using LibraryApp.Models;

namespace LibraryApp.Services;

public interface IDataService
{
    AppData Load();
    void Save(AppData data);
}
