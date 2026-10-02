using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Data;
using StudentManagementSystem.Models;

namespace StudentManagementSystem.Controllers
{
    public class CoursesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CoursesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var courses = await _context.Courses
                .Include(c => c.Department)
                .ToListAsync();
            return View(courses);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var course = await _context.Courses
                .Include(c => c.Department)
                .Include(c => c.Enrollments)
                    .ThenInclude(e => e.Student)
                .FirstOrDefaultAsync(c => c.CourseId == id);

            if (course == null) return NotFound();
            return View(course);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Departments = new SelectList(await _context.Departments.ToListAsync(), "DepartmentId", "DepartmentName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Course course)
        {
            if (await _context.Courses.AnyAsync(c => c.CourseCode == course.CourseCode))
            {
                ModelState.AddModelError("CourseCode", "This course code already exists.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(course);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Course created successfully.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Departments = new SelectList(await _context.Departments.ToListAsync(), "DepartmentId", "DepartmentName", course.DepartmentId);
            return View(course);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound();

            ViewBag.Departments = new SelectList(await _context.Departments.ToListAsync(), "DepartmentId", "DepartmentName", course.DepartmentId);
            return View(course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Course course)
        {
            if (id != course.CourseId) return NotFound();

            if (await _context.Courses.AnyAsync(c => c.CourseCode == course.CourseCode && c.CourseId != id))
            {
                ModelState.AddModelError("CourseCode", "This course code already exists.");
            }

            if (ModelState.IsValid)
            {
                _context.Update(course);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Course updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Departments = new SelectList(await _context.Departments.ToListAsync(), "DepartmentId", "DepartmentName", course.DepartmentId);
            return View(course);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var course = await _context.Courses
                .Include(c => c.Department)
                .FirstOrDefaultAsync(c => c.CourseId == id);
            if (course == null) return NotFound();
            return View(course);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            bool hasEnrollments = await _context.Enrollments.AnyAsync(e => e.CourseId == id);
            if (hasEnrollments)
            {
                TempData["ErrorMessage"] = "Cannot delete course: students are currently enrolled in it.";
                return RedirectToAction(nameof(Index));
            }

            var course = await _context.Courses.FindAsync(id);
            if (course != null)
            {
                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Course deleted successfully.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
