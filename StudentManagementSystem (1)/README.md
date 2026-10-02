# Student Management System — ASP.NET Core MVC + SQL Server

A fully functional, ready-to-run student record management system built with ASP.NET Core 8 (MVC), Entity Framework Core, and SQL Server.

## Features
- Student CRUD (Create, Read, Update, Delete) with search, filter, and pagination
- Department management
- Course management
- Student-to-course enrollment with semester and grade tracking
- Dashboard with live statistics
- Login/logout authentication (cookie-based)
- Server-side AND client-side validation
- Clean, responsive Bootstrap 5 UI

---

## STEP-BY-STEP SETUP GUIDE

### 1. Install Required Software (if not already done)

| Tool | Download Link |
|---|---|
| .NET 8 SDK | https://dotnet.microsoft.com/download/dotnet/8.0 |
| Visual Studio 2022 Community (select "ASP.NET and web development" workload) | https://visualstudio.microsoft.com/downloads/ |
| SQL Server 2022 Express | https://www.microsoft.com/en-us/sql-server/sql-server-downloads |
| SQL Server Management Studio (SSMS) | https://learn.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms |

Verify .NET installed correctly:
```bash
dotnet --version
```

### 2. Set Up the Database

1. Open **SSMS**, connect to `localhost\SQLEXPRESS` (Windows Authentication).
2. Right-click **Databases** → **New Database** → name it `StudentManagementDB` → OK.
3. Open a **New Query** window, open `Database/01_CreateDatabase.sql` from this package, paste its contents, and press **F5** to execute.
4. You should see "Database setup completed successfully!" in the output pane.

### 3. Open the Project

1. Copy the entire `Project/` folder to your preferred location (e.g., `C:\Projects\StudentManagementSystem`).
2. Open **Visual Studio 2022** → **Open a Project or Solution** → select `StudentManagementSystem.csproj` inside the `Project` folder.
3. Visual Studio will automatically restore NuGet packages. If not, right-click the project → **Restore NuGet Packages**.

### 4. Verify Your Connection String

Open `appsettings.json` and confirm the `Server` name matches your SQL Server instance:
```json
"DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=StudentManagementDB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
```
> If you installed SQL Server with a different instance name, replace `localhost\SQLEXPRESS` accordingly (check in SSMS's connection dialog — that's the exact name to use here).

### 5. Set the Admin Login Password

The seed script creates a placeholder admin user, but you need a real BCrypt password hash. Easiest method:

**Option A — Use an online BCrypt generator (quick, for local dev only):**
1. Go to https://bcrypt-generator.com/
2. Enter your desired password (e.g., `Admin@123`), select **10 rounds**, click **Generate Hash**.
3. Copy the generated hash.
4. Open `Database/02_SetAdminPassword.sql`, paste the hash in place of `PASTE_YOUR_BCRYPT_HASH_HERE`, then run it in SSMS.

**Option B — Generate it from C# (more secure, no third-party site):**
Create a throwaway console app:
```bash
dotnet new console -n HashGen
cd HashGen
dotnet add package BCrypt.Net-Next
```
Replace `Program.cs` contents with:
```csharp
Console.Write("Enter password to hash: ");
string password = Console.ReadLine()!;
string hash = BCrypt.Net.BCrypt.HashPassword(password);
Console.WriteLine("Hash: " + hash);
```
Run `dotnet run`, copy the printed hash, and use it in Step 5 above.

### 6. Run the Application

In Visual Studio, press **F5** (or click the green "Run" button). Your browser will open automatically to the login page.

Or from the command line, inside the `Project` folder:
```bash
dotnet run
```

Log in with:
- **Username:** `admin`
- **Password:** whatever you set in Step 5

---

## Project Structure

```
Project/
├── Controllers/          → Request handling logic (Students, Courses, Departments, Account, Home)
├── Models/               → Data classes (Student, Course, Department, Enrollment, User)
├── Data/                 → ApplicationDbContext (EF Core database connection)
├── Views/                → Razor (.cshtml) pages for each controller
│   ├── Shared/_Layout.cshtml  → Main page template with navigation
│   └── Account/Login.cshtml   → Login page
├── wwwroot/css/site.css  → Custom styling
├── Program.cs            → App startup, authentication & DB configuration
├── appsettings.json      → Connection string & settings
└── StudentManagementSystem.csproj → Project file with NuGet dependencies
```

## How the Architecture Works (for your understanding)

This follows the **MVC (Model-View-Controller)** pattern:
- **Models** define the shape of your data (a Student has a name, email, etc.) and validation rules.
- **Views** are the HTML templates (Razor syntax) that display data to the user.
- **Controllers** sit in between — they receive a browser request, talk to the database (via `ApplicationDbContext`), and decide which View to render with which data.

**Entity Framework Core** is the ORM (Object-Relational Mapper) — it lets you write C# code (`_context.Students.Where(...)`) instead of raw SQL, and EF Core translates it into SQL Server queries behind the scenes.

**Authentication** uses ASP.NET Core's built-in Cookie Authentication — when you log in, a secure cookie is issued; the `[Authorize]` attribute on `HomeController` checks for that cookie on every request to protected pages.

## Extending This Project (Ideas for Practice)

- Add role-based authorization (e.g., only "Admin" can delete records, "Staff" can only view/add)
- Add file upload for student photos
- Add Excel/PDF export for student lists
- Add a REST API layer (`[ApiController]`) so a mobile app could consume the same data
- Add unit tests using xUnit for the controllers

## Troubleshooting

| Problem | Solution |
|---|---|
| "Cannot open database" error | Check SQL Server service is running (Services app → SQL Server (SQLEXPRESS) → Start) |
| Login page never accepts password | Make sure you replaced the placeholder hash (Step 5) — the seed value won't work |
| NuGet packages won't restore | Right-click solution → Restore NuGet Packages, or run `dotnet restore` in terminal |
| Port already in use | Change the port in `Properties/launchSettings.json` or close the other running app |
