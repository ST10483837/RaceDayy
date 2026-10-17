using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayy.Data;
using RaceDayy.Models;
using RaceDayy.Models.ViewModels;

namespace RaceDayy.Controllers
{
    [Authorize(Roles = "Organizer")]
    public class OrganizerDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrganizerDashboardController(ApplicationDbContext context,
                                            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var organizer = await _context.Organizers
                .FirstOrDefaultAsync(o => o.UserId == user.Id);

            var model = new OrganizerDashboardViewModel();

            if (organizer != null)
            {
                var events = await _context.Events
                    .Include(e => e.Categories)
                    .ThenInclude(c => c.Enrollments)
                    .Where(e => e.OrganizerID == organizer.OrganizerID)
                    .ToListAsync();

                model.TotalEvents = events.Count;
                model.TotalCategories = events.Sum(e => e.Categories.Count);
                model.TotalEnrollments = events.Sum(e => e.Categories.Sum(c => c.Enrollments.Count));

                model.Events = events.Select(e => new EventSummary
                {
                    EventID = e.EventID,
                    EventName = e.EventName,
                    EventDate = e.EventDate,
                    Status = e.Status,
                    CategoryCount = e.Categories.Count,
                    EnrollmentCount = e.Categories.Sum(c => c.Enrollments.Count)
                }).ToList();
            }

            return View(model);
        }
    }
}