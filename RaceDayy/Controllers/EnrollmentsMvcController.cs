using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayy.Data;
using RaceDayy.Models;

namespace RaceDayy.Controllers
{
    [Authorize] // User must be logged in
    public class EnrollmentsMvcController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EnrollmentsMvcController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /EnrollmentsMvc/MyEnrollments
        public async Task<IActionResult> MyEnrollments()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var participant = await _context.Participants.FirstOrDefaultAsync(p => p.UserId == user.Id);

            if (participant == null)
            {
                return View(new List<Enrollment>()); // Return empty list if they haven't enrolled in anything yet
            }

            var enrollments = await _context.Enrollments
                .Include(e => e.Category)
                .ThenInclude(c => c.Event)
                .Where(e => e.ParticipantID == participant.ParticipantID)
                .ToListAsync();

            return View(enrollments);
        }

        // POST: /EnrollmentsMvc/Enroll
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enroll(int categoryId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            // 1. SAFETY CHECK: Does the Category exist?
            var categoryExists = await _context.Categories.AnyAsync(c => c.CategoryID == categoryId);
            if (!categoryExists)
            {
                TempData["ErrorMessage"] = "Cannot enroll: This category does not exist in the database yet.";
                return RedirectToAction("Index", "EventsMvc");
            }

            // 2. Find or create the participant profile
            var participant = await _context.Participants.FirstOrDefaultAsync(p => p.UserId == user.Id);
            if (participant == null)
            {
                participant = new Participant
                {
                    UserId = user.Id,
                    Name = user.FullName,
                    Email = user.Email ?? ""
                };
                _context.Participants.Add(participant);
                await _context.SaveChangesAsync();
            }

            // 3. Check if already enrolled
            var alreadyEnrolled = await _context.Enrollments
                .AnyAsync(e => e.ParticipantID == participant.ParticipantID && e.CategoryID == categoryId);

            if (!alreadyEnrolled)
            {
                var enrollment = new Enrollment
                {
                    ParticipantID = participant.ParticipantID,
                    CategoryID = categoryId,
                    Status = "Registered",
                    PaymentStatus = "Pending"
                };
                _context.Enrollments.Add(enrollment);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Successfully enrolled!";
            }
            else
            {
                TempData["ErrorMessage"] = "You are already enrolled in this category.";
            }

            return RedirectToAction(nameof(MyEnrollments));
        }
    }
}