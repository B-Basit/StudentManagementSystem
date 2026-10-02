-- ============================================================
-- Student Management System - Database Setup Script
-- Run this in SSMS after creating StudentManagementDB
-- ============================================================

USE StudentManagementDB;
GO

-- ============================================================
-- Table: Departments
-- ============================================================
CREATE TABLE Departments (
    DepartmentId INT IDENTITY(1,1) PRIMARY KEY,
    DepartmentName NVARCHAR(100) NOT NULL,
    DepartmentCode NVARCHAR(10) NOT NULL UNIQUE,
    CreatedDate DATETIME DEFAULT GETDATE()
);
GO

-- ============================================================
-- Table: Courses
-- ============================================================
CREATE TABLE Courses (
    CourseId INT IDENTITY(1,1) PRIMARY KEY,
    CourseName NVARCHAR(150) NOT NULL,
    CourseCode NVARCHAR(20) NOT NULL UNIQUE,
    Credits INT NOT NULL DEFAULT 3,
    DepartmentId INT NOT NULL,
    CreatedDate DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Courses_Departments FOREIGN KEY (DepartmentId)
        REFERENCES Departments(DepartmentId)
);
GO

-- ============================================================
-- Table: Students
-- ============================================================
CREATE TABLE Students (
    StudentId INT IDENTITY(1,1) PRIMARY KEY,
    RegistrationNumber NVARCHAR(30) NOT NULL UNIQUE,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(150) NOT NULL UNIQUE,
    PhoneNumber NVARCHAR(20) NULL,
    DateOfBirth DATE NOT NULL,
    Gender NVARCHAR(10) NOT NULL,
    Address NVARCHAR(300) NULL,
    DepartmentId INT NOT NULL,
    EnrollmentDate DATE NOT NULL DEFAULT GETDATE(),
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE(),
    ModifiedDate DATETIME NULL,
    CONSTRAINT FK_Students_Departments FOREIGN KEY (DepartmentId)
        REFERENCES Departments(DepartmentId)
);
GO

-- ============================================================
-- Table: Enrollments (Student <-> Course, many-to-many)
-- ============================================================
CREATE TABLE Enrollments (
    EnrollmentId INT IDENTITY(1,1) PRIMARY KEY,
    StudentId INT NOT NULL,
    CourseId INT NOT NULL,
    Semester NVARCHAR(20) NOT NULL,       -- e.g. 'Fall 2026'
    Grade NVARCHAR(5) NULL,                -- e.g. 'A', 'B+', NULL if in progress
    EnrollmentDate DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Enrollments_Students FOREIGN KEY (StudentId)
        REFERENCES Students(StudentId) ON DELETE CASCADE,
    CONSTRAINT FK_Enrollments_Courses FOREIGN KEY (CourseId)
        REFERENCES Courses(CourseId),
    CONSTRAINT UQ_Student_Course_Semester UNIQUE (StudentId, CourseId, Semester)
);
GO

-- ============================================================
-- Table: Users (for login / authentication)
-- ============================================================
CREATE TABLE Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(256) NOT NULL,
    Role NVARCHAR(20) NOT NULL DEFAULT 'Staff',   -- 'Admin' or 'Staff'
    CreatedDate DATETIME DEFAULT GETDATE()
);
GO

-- ============================================================
-- Indexes for performance
-- ============================================================
CREATE INDEX IX_Students_DepartmentId ON Students(DepartmentId);
CREATE INDEX IX_Students_RegistrationNumber ON Students(RegistrationNumber);
CREATE INDEX IX_Enrollments_StudentId ON Enrollments(StudentId);
CREATE INDEX IX_Enrollments_CourseId ON Enrollments(CourseId);
GO

-- ============================================================
-- Sample Seed Data
-- ============================================================

INSERT INTO Departments (DepartmentName, DepartmentCode) VALUES
('Computer Science', 'CS'),
('Software Engineering', 'SE'),
('Business Administration', 'BBA'),
('Electrical Engineering', 'EE');
GO

INSERT INTO Courses (CourseName, CourseCode, Credits, DepartmentId) VALUES
('Introduction to Programming', 'CS101', 3, 1),
('Data Structures & Algorithms', 'CS201', 4, 1),
('Database Systems', 'CS301', 3, 1),
('Software Engineering Principles', 'SE101', 3, 2),
('Web Application Development', 'SE201', 4, 2),
('Principles of Management', 'BBA101', 3, 3),
('Circuit Analysis', 'EE101', 3, 4);
GO

INSERT INTO Students
(RegistrationNumber, FirstName, LastName, Email, PhoneNumber, DateOfBirth, Gender, Address, DepartmentId, EnrollmentDate)
VALUES
('2024-CS-001', 'Ahmed', 'Khan', 'ahmed.khan@example.com', '0300-1234567', '2002-05-14', 'Male', 'Islamabad, Pakistan', 1, '2024-09-01'),
('2024-SE-002', 'Ayesha', 'Malik', 'ayesha.malik@example.com', '0301-2345678', '2003-02-20', 'Female', 'Rawalpindi, Pakistan', 2, '2024-09-01'),
('2024-BBA-003', 'Bilal', 'Ahmed', 'bilal.ahmed@example.com', '0302-3456789', '2002-11-09', 'Male', 'Lahore, Pakistan', 3, '2024-09-01');
GO

INSERT INTO Enrollments (StudentId, CourseId, Semester, Grade) VALUES
(1, 1, 'Fall 2024', 'A'),
(1, 2, 'Spring 2025', 'B+'),
(2, 4, 'Fall 2024', 'A-'),
(2, 5, 'Spring 2025', NULL),
(3, 6, 'Fall 2024', 'B');
GO

-- Default admin user (password = "Admin@123" - will be hashed properly by the app on first run)
-- This is just a placeholder; see SeedAdmin instructions in README
INSERT INTO Users (Username, PasswordHash, Role) VALUES
('admin', 'PLACEHOLDER_WILL_BE_REPLACED', 'Admin');
GO

PRINT 'Database setup completed successfully!';
