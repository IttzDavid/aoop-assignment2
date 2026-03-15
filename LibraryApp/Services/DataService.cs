using System;
using System.IO;
using System.Text.Json;
using LibraryApp.Models;

namespace LibraryApp.Services;

public class DataService : IDataService
{
    private readonly string _filePath;

    public DataService(string? filePath = null)
    {
        _filePath = filePath ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data.json");
    }

    public AppData Load()
    {
        if (!File.Exists(_filePath))
            return CreateDefaultData();
        try
        {
            var json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<AppData>(json) ?? CreateDefaultData();
        }
        catch
        {
            return CreateDefaultData();
        }
    }

    public void Save(AppData data)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        var json = JsonSerializer.Serialize(data, options);
        File.WriteAllText(_filePath, json);
    }

    private static AppData CreateDefaultData()
    {
        return new AppData
        {
            Books = new System.Collections.Generic.List<Book>
            {
                new() { Id = "book1", Title = "The Great Gatsby", Author = "F. Scott Fitzgerald", ISBN = "978-0743273565", Description = "A novel set in the Jazz Age on Long Island.", IsAvailable = true },
                new() { Id = "book2", Title = "To Kill a Mockingbird", Author = "Harper Lee", ISBN = "978-0061935466", Description = "A story of racial injustice and the loss of innocence.", IsAvailable = true },
                new() { Id = "book3", Title = "1984", Author = "George Orwell", ISBN = "978-0451524935", Description = "A dystopian novel about totalitarianism.", IsAvailable = true },
                new() { Id = "book4", Title = "Pride and Prejudice", Author = "Jane Austen", ISBN = "978-0141439518", Description = "A romantic novel of manners.", IsAvailable = true },
                new() { Id = "book5", Title = "The Catcher in the Rye", Author = "J.D. Salinger", ISBN = "978-0316769174", Description = "A story of teenage angst and alienation.", IsAvailable = true },
            },
            Members = new System.Collections.Generic.List<Member>
            {
                new() { Id = "member1", Username = "alice", Password = "password123", Name = "Alice Smith" },
                new() { Id = "member2", Username = "bob", Password = "password456", Name = "Bob Johnson" },
            },
            Librarians = new System.Collections.Generic.List<Librarian>
            {
                new() { Id = "lib1", Username = "librarian", Password = "libpass", Name = "Carol Williams" },
            },
            ActiveLoans = new System.Collections.Generic.List<Loan>()
        };
    }
}
