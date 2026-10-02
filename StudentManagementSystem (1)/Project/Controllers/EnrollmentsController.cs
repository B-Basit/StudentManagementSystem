using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Data;
using StudentManagementSystem.Models;

namespace StudentManagementSystem.Controllers
{
    public class EnrollmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EnrollmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Enrollments  (list ALL enrollments across all students)
        public async Task<IActionResult> Index(string? semesterFilter, int? courseFilter)
        {
            var query = _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .AsQueryable();

            if (!string.IsNullOrEmpty(semesterFilter))
            {
                query = query.Where(e => e.Semester == semesterFilter);
            }

            if (courseFilter.HasValue && courseFilter.Value > 0)
            {
                query = query.Where(e => e.CourseId == courseFilter.Value);
            }

            var enrollments = await query
                .OrderByDescending(e => e.EnrollmentDate)
                .ToListAsync();

            ViewBag.Courses = new SelectList(await _context.Courses.ToListAsync(), "CourseId", "CourseName");
            ViewBag.Semesters = await _context.Enrollments
                .Select(e => e.Semester)
                .Distinct()
                .ToListAsync();
            ViewBag.SemesterFilter = semesterFilter;
            ViewBag.CourseFilter = courseFilter;

            return View(enrollments);
        }

        // GET: Enrollments/Create  (standalone enroll form, pick student AND course)
        public async Task<IActionResult> Create()
        {
            ViewBag.Students = new SelectList(await _context.Students.ToListAsync(), "StudentId", "RegistrationNumber");
            ViewBag.Courses = new SelectList(await _context.Courses.ToListAsync(), "CourseId", "CourseName");
            return View();
        }

        // POST: Enrollments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int studentId, int courseId, string semester, string? grade)
        {
            bool alreadyEnrolled = await _context.Enrollments.AnyAsync(e =>
                e.StudentId == studentId && e.CourseId == courseId && e.Semester == semester);

            if (alreadyEnrolled)
            {
                TempData["ErrorMessage"] = "This student is already enrolled in this course for this semester.";
            }
            else
            {
                var enrollment = new Enrollment
                {
                    StudentId = studentId,
                    CourseId = courseId,
                    Semester = semester,
                    Grade = string.IsNullOrWhiteSpace(grade) ? null : grade,
                    EnrollmentDate = DateTime.Now
                };
                _context.Enrollments.Add(enrollment);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Enrollment created successfully.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Students = new SelectList(await _context.Students.ToListAsync(), "StudentId", "RegistrationNumber");
            ViewBag.Courses = new SelectList(await _context.Courses.ToListAsync(), "CourseId", "CourseName");
            return View();
        }

        // GET: Enrollments/Edit/5  (update grade)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var enrollment = await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.EnrollmentId == id);

            if (enrollment == null) return NotFound();
            return View(enrollment);
        }

        // POST: Enrollments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string semester, string? grade)
        {
            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment == null) return NotFound();

            enrollment.Semester = semester;
            enrollment.Grade = string.IsNullOrWhiteSpace(grade) ? null : grade;

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Enrollment updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Enrollments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var enrollment = await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.EnrollmentId == id);

            if (enrollment == null) return NotFound();
            return View(enrollment);
        }

        // POST: Enrollments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment != null)
            {
                _context.Enrollments.Remove(enrollment);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Enrollment removed successfully.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
