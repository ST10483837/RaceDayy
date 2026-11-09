using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayy.Data;
using RaceDayy.Models;

namespace RaceDayy.Controllers
{
    [Authorize(Roles = "Admin,Organizer")]
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CategoriesController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: /Categories?eventId=5
        public async Task<IActionResult> Index(int eventId)
        {
            var ev = await _db.Events
                .Include(e => e.Categories)
                .FirstOrDefaultAsync(e => e.EventID == eventId);

            if (ev == null) return NotFound();

            ViewBag.EventName = ev.EventName;
            ViewBag.EventId = ev.EventID;

            return View(ev.Categories.ToList());
        }

        // POST: /Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                _db.Categories.Add(category);
                await _db.SaveChangesAsync();
                TempData["Success"] = "Category added successfully.";
            }
            return RedirectToAction(nameof(Index), new { eventId = category.EventID });
        }

        // POST: /Categories/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _db.Categories.FindAsync(id);
            if (category != null)
            {
                _db.Categories.Remove(category);
                await _db.SaveChangesAsync();
                TempData["Success"] = "Category deleted.";
            }
            return RedirectToAction(nameof(Index), new { eventId = category?.EventID });
        }
    }
}