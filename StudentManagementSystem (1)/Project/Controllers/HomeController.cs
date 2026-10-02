using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Data;

namespace StudentManagementSystem.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalStudents = await _context.Students.CountAsync();
            ViewBag.ActiveStudents = await _context.Students.CountAsync(s => s.IsActive);
            ViewBag.TotalCourses = await _context.Courses.CountAsync();
            ViewBag.TotalDepartments = await _context.Departments.CountAsync();

            ViewBag.StudentsByDepartment = await _context.Departments
                .Select(d => new
                {
                    DepartmentName = d.DepartmentName,
                    Count = d.Students!.Count()
                })
                .ToListAsync();

            ViewBag.RecentStudents = await _context.Students
                .Include(s => s.Department)
                .OrderByDescending(s => s.CreatedDate)
                .Take(5)
                .ToListAsync();

            return View();
        }

        public IActionResult Privacy() => View();
    }
}
