using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayy.Data;
using RaceDayy.Models;
using RaceDayy.Services;

namespace RaceDayy.Controllers
{
    [Authorize]
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _users;
        private readonly IBlobStorageService _blobs;

        public EventsController(ApplicationDbContext db, UserManager<ApplicationUser> users, IBlobStorageService blobs)
        {
            _db = db;
            _users = users;
            _blobs = blobs;
        }

        // ==========================================
        // MVC WEB PAGES (Views)
        // ==========================================

        // GET: /Events
        public async Task<IActionResult> Index()
        {
            ViewBag.UserId = _users.GetUserId(User);
            var events = await _db.Events
                .Include(e => e.Organizer)
                .AsNoTracking()
                .OrderBy(e => e.EventDate)
                .ToListAsync();

            return View(events);
        }

        // GET: /Events/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var ev = await _db.Events
                .Include(e => e.Organizer).ThenInclude(o => o.User)
                .Include(e => e.Categories)
                .FirstOrDefaultAsync(m => m.EventID == id);

            if (ev == null) return NotFound();

            var userId = _users.GetUserId(User);
            var organizer = await _db.Organizers.FirstOrDefaultAsync(o => o.UserId == userId);

            ViewBag.IsOwner = organizer != null && ev.OrganizerID == organizer.OrganizerID;
            ViewBag.IsEnrolled = false; // Placeholder for when we build enrollments

            return View(ev);
        }

        // GET: /Events/Create
        [Authorize(Roles = "Admin,Organizer")]
        public IActionResult Create()
        {
            return View(new Event { EventDate = DateTime.Today.AddDays(30).AddHours(8) });
        }

        // POST: /Events/Create
        [HttpPost, ValidateAntiForgeryToken, RequestSizeLimit(5_000_000)]
        [Authorize(Roles = "Admin,Organizer")]
        public async Task<IActionResult> Create([Bind("EventName,Description,EventDate,Distance,EventType")] Event ev, IFormFile? bannerFile)
        {
            var hasBanner = bannerFile is { Length: > 0 };

            if (hasBanner)
            {
                var error = _blobs.Validate(bannerFile!);
                if (error != null) ModelState.AddModelError("bannerFile", error);
            }

            if (!ModelState.IsValid) return View(ev);

            // --- FIX 1: FIND OR CREATE ORGANIZER ---
            var userId = _users.GetUserId(User);
            var organizer = await _db.Organizers.FirstOrDefaultAsync(o => o.UserId == userId);

            if (organizer == null)
            {
                var user = await _users.FindByIdAsync(userId);
                organizer = new Organizer
                {
                    UserId = userId,
                    Name = $"{user.FirstName} {user.LastName}",
                    Email = user.Email ?? ""
                };
                _db.Organizers.Add(organizer);
                await _db.SaveChangesAsync();
            }
            ev.OrganizerID = organizer.OrganizerID;

            // --- FIX 2: FIND OR CREATE A DEFAULT VENUE ---
            var venue = await _db.Venues.FirstOrDefaultAsync();
            if (venue == null)
            {
                venue = new Venue
                {
                    VenueName = "Default Stadium",
                    Address = "123 Main Street",
                    City = "Johannesburg",
                    Country = "South Africa",
                    Capacity = 5000
                };
                _db.Venues.Add(venue);
                await _db.SaveChangesAsync();
            }
            ev.VenueID = venue.VenueID;
            // -----------------------------------------

            BlobUploadResult? uploaded = null;

            try
            {
                if (hasBanner)
                {
                    uploaded = await _blobs.UploadBannerAsync(bannerFile!);
                    ev.BannerBlobName = uploaded.BlobName;
                    ev.BannerImageUrl = uploaded.Url;
                }

                _db.Events.Add(ev);
                await _db.SaveChangesAsync();
            }
            catch
            {
                if (uploaded != null) await _blobs.DeleteAsync(uploaded.BlobName);
                throw;
            }

            TempData["Success"] = "Event created. Now add the categories for it.";
            return RedirectToAction("Index", "Categories", new { eventId = ev.EventID });
        }

        // GET: /Events/Edit/5
        [Authorize(Roles = "Admin,Organizer")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var ev = await _db.Events.FindAsync(id);
            if (ev == null) return NotFound();
            return View(ev);
        }

        // POST: /Events/Edit/5
        [HttpPost, ValidateAntiForgeryToken, RequestSizeLimit(5_000_000)]
        [Authorize(Roles = "Admin,Organizer")]
        public async Task<IActionResult> Edit(int id, [Bind("EventID,EventName,Description,EventDate,Distance,EventType")] Event ev, IFormFile? bannerFile, bool removeBanner)
        {
            if (id != ev.EventID) return NotFound();
            var existingEvent = await _db.Events.FindAsync(id);
            if (existingEvent == null) return NotFound();

            var hasBanner = bannerFile is { Length: > 0 };
            if (hasBanner)
            {
                var error = _blobs.Validate(bannerFile!);
                if (error != null) ModelState.AddModelError("bannerFile", error);
            }

            if (!ModelState.IsValid) return View(ev);

            existingEvent.EventName = ev.EventName;
            existingEvent.Description = ev.Description;
            existingEvent.EventDate = ev.EventDate;
            existingEvent.Distance = ev.Distance;
            existingEvent.EventType = ev.EventType;

            var oldBlobName = existingEvent.BannerBlobName;
            BlobUploadResult? uploaded = null;

            try
            {
                if (hasBanner)
                {
                    uploaded = await _blobs.UploadBannerAsync(bannerFile!);
                    existingEvent.BannerBlobName = uploaded.BlobName;
                    existingEvent.BannerImageUrl = uploaded.Url;
                }
                else if (removeBanner)
                {
                    existingEvent.BannerBlobName = null;
                    existingEvent.BannerImageUrl = null;
                }

                _db.Update(existingEvent);
                await _db.SaveChangesAsync();
            }
            catch
            {
                if (uploaded != null) await _blobs.DeleteAsync(uploaded.BlobName);
                throw;
            }

            if ((uploaded != null || removeBanner) && oldBlobName != null)
                await _blobs.DeleteAsync(oldBlobName);

            TempData["Success"] = "Event updated.";
            return RedirectToAction(nameof(Details), new { id });
        }

        // GET: /Events/Delete/5
        [Authorize(Roles = "Admin,Organizer")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var ev = await _db.Events.FindAsync(id);
            if (ev == null) return NotFound();
            return View(ev);
        }

        // POST: /Events/Delete/5
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Organizer")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ev = await _db.Events.FindAsync(id);
            if (ev == null) return NotFound();

            if (await _db.Enrollments.AnyAsync(e => e.Category.EventID == id))
            {
                TempData["Error"] = "This event has enrolments and cannot be deleted.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var blobName = ev.BannerBlobName;
            _db.Events.Remove(ev);
            await _db.SaveChangesAsync();

            if (blobName != null) await _blobs.DeleteAsync(blobName);

            TempData["Success"] = "Event deleted.";
            return RedirectToAction(nameof(Index));
        }

        // ==========================================
        // API ENDPOINTS (Swagger)
        // ==========================================

        [HttpGet("api/events")]
        [AllowAnonymous] // Allows anyone to view API events
        public async Task<IActionResult> GetAllApi()
        {
            var events = await _db.Events.Include(e => e.Venue).ToListAsync();
            return Ok(events);
        }

        [HttpGet("api/events/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByIdApi(int id)
        {
            var ev = await _db.Events.Include(e => e.Categories).FirstOrDefaultAsync(e => e.EventID == id);
            if (ev == null) return NotFound();
            return Ok(ev);
        }
    }
}