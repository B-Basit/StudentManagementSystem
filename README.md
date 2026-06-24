1. EXECUTIVE SUMMARY

The Student Management System is a full-stack web application designed to digitize and centralize the academic record-keeping process for an educational institution. It replaces manual, paper-based or spreadsheet-driven student tracking with a structured, relational database-backed system that allows administrative staff to manage student profiles, departments, courses, and course enrollments through an intuitive web interface.
The system was built end-to-end using ASP.NET Core MVC — a modern, industry-standard Microsoft framework — paired with Microsoft SQL Server as the relational database engine. It demonstrates a complete software development lifecycle: requirement identification, relational database design, server-side application logic, secure authentication, and a responsive, user-friendly front end.
Key Highlights
•	Complete CRUD (Create, Read, Update, Delete) functionality for Students, Courses, and Departments
•	Relational database with 5 normalized tables and proper foreign key relationships
•	Secure, cookie-based authentication system with hashed passwords (BCrypt)
•	Search, filtering, and pagination for efficient record navigation
•	Student-to-course enrollment system with semester and grade tracking
•	Live dashboard with real-time statistics and data visualization
•	Server-side AND client-side validation for data integrity
•	Clean, responsive UI built with Bootstrap 5

2. INTRODUCTION

2.1 Background
Educational institutions, regardless of size, need a reliable way to track student information: personal details, departmental affiliation, course enrollments, and academic performance. Many small institutions still rely on Excel sheets or paper registers, which are prone to data duplication, loss, and inconsistency, and don't scale well as student numbers grow.
This project addresses that gap by providing a centralized, database-driven system accessible through a standard web browser, with built-in safeguards against duplicate or invalid data entry.
2.2 Problem Statement
•	Manual record-keeping (paper/Excel) is error-prone and difficult to search or update at scale.
•	There is no single source of truth linking students, the departments they belong to, and the courses they are enrolled in.
•	Tracking academic progress (grades, semester history) becomes unmanageable without a structured database.
•	Unauthorized or accidental data changes are hard to prevent without proper access control.
2.3 Proposed Solution
A web-based Student Management System with a normalized relational database backend, where:
•	All student, department, course, and enrollment data is stored in a single, structured SQL Server database.
•	Only authenticated staff/administrators can access and modify records, via secure login.
•	Business rules (e.g., no duplicate registration numbers, no duplicate enrollments) are enforced at both the application and database level.
•	The interface is accessible from any modern web browser, requiring no software installation for end users.

3. PROJECT OBJECTIVES

1.	Design and implement a normalized relational database schema for student academic records.
2.	Build a secure, role-aware authentication system to control access to sensitive data.
3.	Develop a complete CRUD interface for managing students, courses, and departments.
4.	Implement a many-to-many enrollment system linking students to courses with semester and grade tracking.
5.	Provide search, filtering, and pagination to make large datasets easy to navigate.
6.	Apply both client-side and server-side validation to maintain data integrity.
7.	Demonstrate the MVC (Model-View-Controller) architectural pattern in a real, working application.
8.	Build a dashboard that surfaces key statistics at a glance for administrative decision-making.

4. SYSTEM ARCHITECTURE

4.1 Architectural Pattern: MVC
The application follows the Model-View-Controller (MVC) architectural pattern, which separates concerns into three distinct layers:
Layer	Responsibility
Model	Defines the shape of data (Student, Course, Department, Enrollment, User) and validation rules. Maps directly to database tables via Entity Framework Core.
View	Razor (.cshtml) templates that render HTML to the browser — forms, tables, dashboards.
Controller	Receives HTTP requests, applies business logic, talks to the database via the DbContext, and selects which View to return.

4.2 Request Flow Diagram (Textual)
  Browser (User)
       |  HTTP Request (e.g., GET /Students/Index)
       v
  Controller (StudentsController)
       |  Calls ApplicationDbContext
       v
  Entity Framework Core (ORM)
       |  Translates to SQL
       v
  SQL Server Database
       |  Returns data
       v
  Controller passes data to View
       |
       v
  Razor View renders HTML
       |
       v
  Browser displays the page

4.3 Three-Tier Architecture
•	Presentation Tier: Razor Views + Bootstrap 5 (what the user sees and interacts with).
•	Application/Logic Tier: ASP.NET Core Controllers (business rules, validation, authentication).
•	Data Tier: SQL Server database accessed through Entity Framework Core (ApplicationDbContext).

5. TECHNOLOGY STACK

Layer	Technology	Purpose / Reason for Choice
Backend Framework	ASP.NET Core 8 (MVC)	Modern, cross-platform, high-performance Microsoft web framework; industry-relevant and actively maintained.
Programming Language	C#	Strongly-typed, object-oriented language with first-class support in ASP.NET Core.
ORM	Entity Framework Core	Maps C# classes to database tables, eliminating most raw SQL and reducing bugs from manual query-writing.
Database	Microsoft SQL Server (Express)	Robust, free, production-grade relational database; the standard pairing with ASP.NET in enterprise and government systems.
Authentication	ASP.NET Core Cookie Authentication + BCrypt	Secure, built-in session handling; BCrypt ensures passwords are never stored in plain text.
Front-End	Razor Views, Bootstrap 5, Bootstrap Icons	Server-rendered HTML with a clean, responsive, mobile-friendly design system.
IDE	Visual Studio 2022	Industry-standard IDE for .NET development, with integrated debugging and NuGet package management.

6. DATABASE DESIGN

6.1 Entity-Relationship Overview
The database consists of five core tables, normalized to Third Normal Form (3NF) to eliminate data redundancy and maintain integrity:
  Departments  1 ----- *  Students
       |                     |
       |                     | *
       1                     |
       |                     1
       *                     |
    Courses  * ----------- Enrollments
       1                     
                          Users
                    (independent - login only)

6.2 Table Descriptions
Table	Description
Departments	Stores academic departments (e.g., Computer Science, Software Engineering). Each has a unique DepartmentCode.
Courses	Stores courses offered, each linked to one Department, with credit hours and a unique CourseCode.
Students	Stores student personal and academic details, linked to one Department. RegistrationNumber and Email are unique.
Enrollments	A junction (many-to-many) table linking Students to Courses, recording Semester and Grade. Prevents duplicate enrollment in the same course/semester.
Users	Stores login credentials for staff/admin access. Passwords are stored as BCrypt hashes, never in plain text.

6.3 Key Database Design Decisions
•	Normalization (3NF): Department info is stored once in Departments, not duplicated across every Student/Course row — preventing update anomalies.
•	Unique constraints: RegistrationNumber, Email, CourseCode, DepartmentCode, and Username all have unique indexes to prevent duplicate records.
•	Foreign keys with cascading rules: Deleting a Student cascades to remove their Enrollments; deleting a Course is restricted if students are still enrolled (data-loss prevention).
•	Composite unique constraint: (StudentId, CourseId, Semester) ensures a student cannot be enrolled in the same course twice in the same semester.
•	Indexes: Added on frequently-queried columns (DepartmentId, RegistrationNumber, StudentId, CourseId) to keep search and filter operations fast as data grows.

7. KEY FEATURES

Feature	Description
Student Management	Full CRUD: add, view, edit, and delete student records with form validation on registration number, email format, and required fields.
Department Management	Create and manage academic departments; deletion is blocked if students are still assigned, preventing orphaned data.
Course Management	Create and manage courses with credit hours, linked to a department; deletion is blocked if students are currently enrolled.
Enrollment System	Enroll a student into a course for a specific semester; duplicate enrollments in the same semester are automatically rejected.
Search & Filter	Search students by name, registration number, or email; filter by department — all combinable.
Pagination	Large student lists are paginated (10 per page) to keep the interface fast and usable.
Dashboard	Live statistics: total students, active students, total courses, total departments, students-by-department breakdown, and recently added students.
Authentication	Cookie-based login system; passwords hashed with BCrypt (industry-standard, salted hashing — never stored or transmitted in plain text).
Data Validation	Both client-side (instant feedback) and server-side (security-critical) validation on every form.

8. CODE WALKTHROUGH — KEY IMPLEMENTATION HIGHLIGHTS

8.1 Model Validation (Student.cs)
Data annotations on the model enforce rules at the application layer before anything reaches the database:
[Required(ErrorMessage = "Email is required")]
[EmailAddress(ErrorMessage = "Enter a valid email address")]
[StringLength(150)]
public string Email { get; set; } = string.Empty;

8.2 Duplicate-Prevention Logic (StudentsController.cs)
Before saving, the controller explicitly checks for duplicate registration numbers or emails — a safeguard beyond what the database's unique constraint alone communicates to the user:
if (await _context.Students.AnyAsync(s =>
    s.RegistrationNumber == student.RegistrationNumber))
{
    ModelState.AddModelError("RegistrationNumber",
        "This registration number already exists.");
}

8.3 Secure Password Verification (AccountController.cs)
var user = await _context.Users
    .FirstOrDefaultAsync(u => u.Username == model.Username);

if (user == null ||
    !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
{
    ModelState.AddModelError("", "Invalid username or password.");
    return View(model);
}

8.4 Efficient Querying with Eager Loading
Entity Framework Core's Include()/ThenInclude() methods load related data (Department, Enrollments, Course) in a single optimized query rather than triggering separate round-trips per record:
var student = await _context.Students
    .Include(s => s.Department)
    .Include(s => s.Enrollments)
        .ThenInclude(e => e.Course)
    .FirstOrDefaultAsync(s => s.StudentId == id);

9. SECURITY CONSIDERATIONS

•	Password Hashing: All passwords are hashed using BCrypt with automatic salting — even if the database were compromised, original passwords cannot be reverse-engineered.
•	Anti-Forgery Tokens: Every form submission includes a hidden anti-forgery token (@Html.AntiForgeryToken()), validated server-side via [ValidateAntiForgeryToken] to prevent Cross-Site Request Forgery (CSRF) attacks.
•	Parameterized Queries (via EF Core): Entity Framework Core automatically parameterizes all generated SQL, eliminating SQL Injection risk by design — no raw string concatenation is used anywhere in the data layer.
•	Authorization Guarding: The [Authorize] attribute on HomeController ensures unauthenticated users are redirected to the login page before they can access any protected data.
•	Server-Side Validation: Even though client-side validation improves UX, all rules are re-validated on the server — since client-side checks can be bypassed by a malicious user.
•	Least-Privilege Principle (Future Enhancement): The Users table already has a Role column (Admin/Staff), laying the groundwork for role-based authorization to restrict sensitive actions like deletion to Admins only.

10. TESTING APPROACH

Test Case	Steps	Expected Result
Duplicate registration number	Create a student using a registration number that already exists	Form rejects submission; error message shown
Invalid email format	Enter 'notanemail' in the Email field	Validation error shown before submission (client-side)
Duplicate enrollment	Enroll the same student in the same course/semester twice	Second attempt is rejected with a clear message
Delete department with students	Attempt to delete a department that has assigned students	Deletion blocked; warning message shown
Unauthenticated access	Try to access /Home/Index without logging in	Redirected to the Login page
Search functionality	Search for a student by partial name	Matching results returned correctly
Login with wrong password	Enter correct username, incorrect password	Login rejected with generic error (no hint on which field was wrong)

Testing was performed manually through the UI for each CRUD operation across all three main entities (Students, Courses, Departments), covering both the 'happy path' (valid data) and edge cases (duplicates, missing fields, invalid formats).

11. CHALLENGES FACED & SOLUTIONS

Challenge	Solution
Preventing duplicate student records with the same registration number or email	Added explicit application-level checks in the controller in addition to unique database indexes — giving the user a clear error instead of a generic database exception
Preventing accidental data loss (e.g., deleting a department that still has students)	Implemented referential-integrity checks before deletion, blocking the action with a clear warning message
Avoiding duplicate enrollments in the same course/semester	Added a composite unique database constraint on (StudentId, CourseId, Semester) plus an application-level pre-check
Storing passwords securely	Used BCrypt hashing instead of storing or comparing plain-text passwords
Keeping large student lists usable	Implemented server-side pagination (10 records per page) combined with search and department filtering

12. FUTURE ENHANCEMENTS

•	Role-Based Authorization: Restrict delete/edit actions to Admin role only; Staff role limited to view/add.
•	REST API Layer: Expose data via a Web API ([ApiController]) so a future mobile app could consume the same backend.
•	Export to Excel/PDF: Allow staff to export student lists or transcripts directly from the dashboard.
•	Profile Photo Upload: Add file upload support for student profile pictures.
•	Email Notifications: Automatically email students when grades are posted or enrollment is confirmed.
•	Audit Logging: Track who changed which record and when, for full accountability.
•	Unit Testing: Add xUnit test coverage for controller logic and validation rules.
•	Two-Factor Authentication (2FA): Strengthen login security for admin accounts.

13. FREE DEPLOYMENT & HOSTING GUIDE

This section explains how to publish the application online completely free of cost, so it can be demonstrated live (e.g., shared as a link in an interview or portfolio) rather than only run locally.
13.1 Recommended Option: Microsoft Azure (Free Tier)
Azure is the natural hosting choice since the application is built on Microsoft's own ASP.NET Core + SQL Server stack, and Microsoft offers a genuinely free tier for both pieces:
Service	Free Allowance
Azure App Service (Free F1 tier)	60 CPU minutes/day, 1 GB RAM, 1 GB storage — enough to host and demo the web application
Azure SQL Database (Free Offer)	Up to 10 databases per subscription, each with 100,000 vCore-seconds of compute, 32 GB storage, and 32 GB backup storage every month — free for the lifetime of the subscription, not just a trial

13.2 Step-by-Step: Deploying to Azure (Free)
1.	Create a free Microsoft Azure account at azure.microsoft.com (requires a credit/debit card for identity verification only — the resources used in this guide incur no charge).
2.	In the Azure Portal, search for 'SQL Database' and click Create. When the pricing tier banner appears, click 'Apply offer' to use the free SQL Database offer instead of a paid tier.
3.	Configure the database: name it (e.g., StudentManagementDB-Free), create a new SQL Server with a server admin username/password (write these down), choose 'Auto-pause the database until next month' to stay strictly within the free limits.
4.	Once created, go to the SQL Database's 'Connection strings' page and copy the ADO.NET connection string.
5.	In Visual Studio, update appsettings.json's DefaultConnection with this new Azure connection string (replace the placeholder password in the string with your actual SQL admin password).
6.	Run the database creation script (01_CreateDatabase.sql) against the Azure database using SSMS — connect using the server name and credentials from Step 3, then execute the script exactly as done locally.
7.	Back in the Azure Portal, search for 'App Service' and click Create. Choose the Free F1 pricing tier, select Runtime stack: .NET 8, and Region: choose one close to you.
8.	In Visual Studio, right-click the project → Publish → choose 'Azure' → 'Azure App Service' → sign in and select the App Service created in Step 7 → click Publish.
9.	Visual Studio builds and deploys the application; once complete, it opens your live URL automatically (e.g., https://studentmanagementsystem.azurewebsites.net).
10.	Test the live link by logging in with your admin credentials — your application is now publicly accessible, completely free.
13.3 Important Free-Tier Notes
•	The free database auto-pauses once the monthly 100,000 vCore-second allowance is used, then resumes automatically at the start of the next month — ideal for a demo project that isn't under constant heavy load.
•	Disconnect SSMS and other query tools when not actively using them — leaving a connection open prevents the database from auto-pausing and can consume your free allowance faster.
•	The free App Service (F1) tier may 'sleep' after periods of inactivity, causing the first request after idle time to load slowly (a 'cold start') — this is normal and acceptable for demo purposes.
•	There is no time limit on this free tier — unlike many trial offers, it remains free for the lifetime of the Azure subscription, renewing monthly.

13.4 Alternative Free Hosting Options
If Azure's setup feels heavy for a quick demo, or if a different database engine is acceptable, these alternatives are also genuinely free for small projects:
Platform	Notes
Render.com	Generous free tier for web services; supports .NET via Docker; easiest if combined with a free PostgreSQL database (would require switching the EF Core provider from SQL Server to PostgreSQL)
Railway.app	Developer-friendly free tier with simple GitHub-based deployment; similar PostgreSQL consideration as Render
Fly.io	Free allowance suitable for small apps; good global reach; requires Docker familiarity

Recommendation: Since this project was built specifically for SQL Server, Azure (App Service Free tier + Azure SQL Database free offer) is the most natural and lowest-friction choice — it requires no code changes, only a connection string update.



