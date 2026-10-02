using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Data;
using StudentManagementSystem.Models;

namespace StudentManagementSystem.Controllers
{
    public class StudentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Students  (with search + pagination)
        public async Task<IActionResult> Index(string? searchString, int? departmentFilter, int pageNumber = 1)
        {
            int pageSize = 10;

            var studentsQuery = _context.Students
                .Include(s => s.Department)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                studentsQuery = studentsQuery.Where(s =>
                    s.FirstName.Contains(searchString) ||
                    s.LastName.Contains(searchString) ||
                    s.RegistrationNumber.Contains(searchString) ||
                    s.Email.Contains(searchString));
            }

            if (departmentFilter.HasValue && departmentFilter.Value > 0)
            {
                studentsQuery = studentsQuery.Where(s => s.DepartmentId == departmentFilter.Value);
            }

            studentsQuery = studentsQuery.OrderBy(s => s.RegistrationNumber);

            int totalItems = await studentsQuery.CountAsync();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var students = await studentsQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = totalPages;
            ViewBag.SearchString = searchString;
            ViewBag.DepartmentFilter = departmentFilter;
            ViewBag.Departments = new SelectList(await _context.Departments.ToListAsync(), "DepartmentId", "DepartmentName");

            return View(students);
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var student = await _context.Students
                .Include(s => s.Department)
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.Course)
                .FirstOrDefaultAsync(s => s.StudentId == id);

            if (student == null) return NotFound();

            return View(student);
        }

        // GET: Students/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Departments = new SelectList(await _context.Departments.ToListAsync(), "DepartmentId", "DepartmentName");
            return View();
        }

        // POST: Students/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Student student)
        {
            // Custom validation: check duplicate registration number / email
            if (await _context.Students.AnyAsync(s => s.RegistrationNumber == student.RegistrationNumber))
            {
                ModelState.AddModelError("RegistrationNumber", "This registration number already exists.");
            }
            if (await _context.Students.AnyAsync(s => s.Email == student.Email))
            {
                ModelState.AddModelError("Email", "This email is already registered.");
            }

            if (ModelState.IsValid)
            {
                student.CreatedDate = DateTime.Now;
                _context.Add(student);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Student '{student.FullName}' was created successfully.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Departments = new SelectList(await _context.Departments.ToListAsync(), "DepartmentId", "DepartmentName", student.DepartmentId);
            return View(student);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var student = await _context.Students.FindAsync(id);
            if (student == null) return NotFound();

            ViewBag.Departments = new SelectList(await _context.Departments.ToListAsync(), "DepartmentId", "DepartmentName", student.DepartmentId);
            return View(student);
        }

        // POST: Students/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Student student)
        {
            if (id != student.StudentId) return NotFound();

            // Check duplicates excluding current record
            if (await _context.Students.AnyAsync(s => s.RegistrationNumber == student.RegistrationNumber && s.StudentId != id))
            {
                ModelState.AddModelError("RegistrationNumber", "This registration number already exists.");
            }
            if (await _context.Students.AnyAsync(s => s.Email == student.Email && s.StudentId != id))
            {
                ModelState.AddModelError("Email", "This email is already registered.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    student.ModifiedDate = DateTime.Now;
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Student '{student.FullName}' was updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.Students.AnyAsync(s => s.StudentId == student.StudentId))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Departments = new SelectList(await _context.Departments.ToListAsync(), "DepartmentId", "DepartmentName", student.DepartmentId);
            return View(student);
        }

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var student = await _context.Students
                .Include(s => s.Department)
                .FirstOrDefaultAsync(s => s.StudentId == id);

            if (student == null) return NotFound();

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Student record was deleted successfully.";
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Students/Enroll/5  (enroll a student in a course)
        public async Task<IActionResult> Enroll(int? id)
        {
            if (id == null) return NotFound();

            var student = await _context.Students.FindAsync(id);
            if (student == null) return NotFound();

            ViewBag.Student = student;
            ViewBag.Courses = new SelectList(await _context.Courses.ToListAsync(), "CourseId", "CourseName");
            return View();
        }

        // POST: Students/Enroll
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enroll(int studentId, int courseId, string semester)
        {
            bool alreadyEnrolled = await _context.Enrollments.AnyAsync(e =>
                e.StudentId == studentId && e.CourseId == courseId && e.Semester == semester);

            if (alreadyEnrolled)
            {
                TempData["ErrorMessage"] = "Student is already enrolled in this course for this semester.";
            }
            else
            {
                var enrollment = new Enrollment
                {
                    StudentId = studentId,
                    CourseId = courseId,
                    Semester = semester,
                    EnrollmentDate = DateTime.Now
                };
                _context.Enrollments.Add(enrollment);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Student enrolled successfully.";
            }

            return RedirectToAction(nameof(Details), new { id = studentId });
        }
    }
}
