# Library Management App — AOOP Assignment 2

A Library Management application built with **C# / Avalonia** following the **MVVM** pattern.

---

## How to Build and Run

```bash
dotnet build
dotnet run --project LibraryApp
```

### Run Tests

```bash
dotnet test
```

---

## Default Credentials

| Role      | Username    | Password      |
|-----------|-------------|---------------|
| Member    | alice       | password123   |
| Member    | bob         | password456   |
| Librarian | librarian   | libpass       |

---

## Project Structure

```
LibraryApp/
├── Models/          Book, Member, Librarian, Loan, AppData
├── Services/        DataService (JSON), AuthService, LibraryService, PasswordHelper
├── ViewModels/      LoginViewModel, MainViewModel, MemberViewModel, LibrarianViewModel
└── Views/           LoginView, MemberView, LibrarianView (AXAML)

LibraryApp.Tests/
├── UnitTests.cs     7 unit tests (borrow/return/login/add/delete/persistence)
└── UITests.cs       3 headless UI tests (Use Cases I-III)
```

---

## Features

- **Login** — role-based login screen (Member or Librarian)
- **Member Portal** — browse catalog, search/filter, borrow & return books
- **Librarian Portal** — full CRUD book management, active loans overview
- **JSON persistence** — data loaded on start, saved on close or any action
- **Password hashing** — PBKDF2/SHA-256 via `PasswordHelper`
- **Borrowing history** — members can see every book they have returned

---

## SOLID Principles Applied

| Principle | Where |
|-----------|-------|
| **S** – Single Responsibility | Each class has one job: `DataService` only handles persistence, `AuthService` only handles auth, `LibraryService` only handles business logic |
| **O** – Open/Closed | Services depend on `IDataService`, `ILibraryService` interfaces; behaviour can be extended without modifying existing classes |
| **L** – Liskov Substitution | `InMemoryDataService` in tests substitutes `DataService` transparently through `IDataService` |
| **I** – Interface Segregation | `IDataService`, `IAuthService`, `ILibraryService` are focused and minimal |
| **D** – Dependency Inversion | ViewModels and services depend on abstractions (interfaces), not concrete types |

---

## Design Patterns Applied

- **MVVM** — ViewModels expose `ObservableProperty` and `RelayCommand` attributes via `CommunityToolkit.Mvvm`
- **Repository / Service Layer** — `LibraryService` encapsulates all business rules, `DataService` handles persistence
- **Observer** — Avalonia binding system reacts to `INotifyPropertyChanged` raised by CommunityToolkit source generators

---

## Functional Testing Results

### Member Functionality Tests

| Test Case | Steps | Expected | Result |
|-----------|-------|----------|--------|
| **Borrowing Test** | 1. Log in as `alice / password123` 2. Go to "Library Catalog" tab 3. Select a book 4. Click "Borrow Selected" | Book disappears from catalog; appears in "My Loans"; success message shown | ✅ PASS |
| **Return Book Test** | 1. Log in as alice 2. Borrow a book 3. Go to "My Loans" tab 4. Select the book 5. Click "Return Selected" | Book removed from "My Loans"; reappears in "Library Catalog" as available; success message shown | ✅ PASS |

### Librarian Functionality Tests

| Test Case | Steps | Expected | Result |
|-----------|-------|----------|--------|
| **Add Book Test** | 1. Log in as `librarian / libpass` 2. Go to "Full Catalog" tab 3. Fill in Title/Author/ISBN/Description 4. Click "Add Book" | New book appears in librarian's full catalog and in member's catalog | ✅ PASS |
| **Delete Book Test** | 1. Log in as librarian 2. Select a book in the catalog 3. Click "Delete" | Book removed from catalog for both librarian and member views | ✅ PASS |
| **Active Loan Tracking** | 1. Borrow a book as alice 2. Log in as librarian 3. Go to "Active Loans" tab | Loan entry shows alice's name, book title, and borrow date | ✅ PASS |

### System-Level Tests

| Test Case | Steps | Expected | Result |
|-----------|-------|----------|--------|
| **Login Test — Member** | Enter `alice / password123`, click Login | Directed to Member portal with catalog and my loans tabs | ✅ PASS |
| **Login Test — Librarian** | Enter `librarian / libpass`, click Login | Directed to Librarian portal with full catalog and active loans tabs | ✅ PASS |
| **Login Test — Invalid** | Enter wrong credentials, click Login | Error message "Invalid username or password." shown | ✅ PASS |
| **Search/Filter Test** | Log in as member; type title or author in search box | List filters to matching books in real time | ✅ PASS |
| **Data Persistence Test** | Borrow a book; close the app; reopen | Borrowed status and loan data persisted in `data.json` | ✅ PASS |

---

## Automated Tests (11 total)

### Unit Tests (`UnitTests.cs`)

1. `BorrowBook_Success_BookBecomesUnavailable`
2. `ReturnBook_Success_BookBecomesAvailable`
3. `BorrowBook_UnavailableBook_ReturnsFalse`
4. `Login_ValidMember_ReturnsMemberRole`
5. `Login_InvalidCredentials_ReturnsNone`
6. `AddBook_AddsToList`
7. `DeleteBook_RemovesFromList`
8. `DeleteBook_CleansUpMemberBorrowedIds`

### UI / Headless Tests (`UITests.cs`) — Avalonia.Headless.XUnit

1. `UseCase1_MemberLogin_NavigatesToMemberView`
2. `UseCase2_MemberBorrowsBook_BookRemovedFromCatalog`
3. `UseCase3_LibrarianViewsActiveLoans_AfterMemberBorrows`

---

## Bonus Features Implemented

- **Password Hashing** — PBKDF2/SHA-256 with random salt via `PasswordHelper`
- **Borrowing History** — members can view all previously returned books (soft-delete approach)