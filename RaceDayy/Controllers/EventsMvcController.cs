using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayy.Data;
using RaceDayy.Models;

namespace RaceDayy.Controllers
{
    public class EventsMvcController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventsMvcController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /EventsMvc
        public async Task<IActionResult> Index()
        {
            var events = await _context.Events.ToListAsync();
            return View(events);
        }

        // GET: /EventsMvc/Details/5
        public async Task<IActionResult> Details(int id)
        {
            if (id == 0) return NotFound();

            var ev = await _context.Events
                .Include(e => e.Categories)
                .Include(e => e.Venue)
                .FirstOrDefaultAsync(m => m.EventID == id);

            if (ev == null) return NotFound();

            return View(ev);
        }

        // GET: /EventsMvc/Create
        [Authorize(Roles = "Admin,Organizer")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /EventsMvc/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Organizer")]
        public async Task<IActionResult> Create(Event newEvent)
        {
            if (ModelState.IsValid)
            {
                // Since this is Part 2, we will need a Venue and Organizer. 
                // For now, let's link it to the first Venue and the logged-in user's Organizer profile.
                var venue = await _context.Venues.FirstOrDefaultAsync();
                if (venue == null)
                {
                    venue = new Venue { VenueName = "Default Venue", Address = "Default", City = "Default", Country = "SA" };
                    _context.Venues.Add(venue);
                    await _context.SaveChangesAsync();
                }

                newEvent.VenueID = venue.VenueID;
                newEvent.Status = "Upcoming";

                // Try to find the organizer profile
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var organizer = await _context.Organizers.FirstOrDefaultAsync(o => o.UserId == userId);

                if (organizer == null)
                {
                    // If admin is creating, just link to first organizer or create a dummy one
                    organizer = await _context.Organizers.FirstOrDefaultAsync();
                    if (organizer == null)
                    {
                        organizer = new Organizer { Name = "System", Email = "system@raceday.com", UserId = userId ?? "" };
                        _context.Organizers.Add(organizer);
                        await _context.SaveChangesAsync();
                    }
                }

                newEvent.OrganizerID = organizer.OrganizerID;

                _context.Events.Add(newEvent);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(newEvent);
        }
    }
}