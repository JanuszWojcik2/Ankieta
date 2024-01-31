using Info.Data;
using Info.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;
using Text = Info.Models.Text;

namespace Info.Controllers
{
    public class OpinionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OpinionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Opinions
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Index(int? id)
        {
            if (id == null)
            {
                var applicationDbContext = _context.Opinions.Include(o => o.Text).Include(o => o.User);
                return View(await applicationDbContext.ToListAsync());
            } 
            else 
            { 
                var appDbContext = _context.Opinions.Include(o => o.Text).Include(o => o.User).Where(o => o.TextId == id);
                return View(await appDbContext.ToListAsync());
            }
        }

        // GET: Opinions/Details/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Opinions == null)
            {
                return NotFound();
            }

            var opinion = await _context.Opinions
                .Include(o => o.Text)
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.OpinionId == id);
            if (opinion == null)
            {
                return NotFound();
            }

            return View(opinion);
        }

        // GET: Opinions/Create
        [Authorize]
        public IActionResult Create(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Text text = _context.Texts.Find(id);
            if (text == null)
            { 
                return BadRequest(); 
            }    
            ViewData["IdText"] = id;
            ViewData["TextTitle"] = text.Title;
            return View();
        }

        // POST: Opinions/Create
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OpinionId,Comment,Rating,TextId")] Opinion opinion)
        {
            if (ModelState.IsValid)
            {
                opinion.AddedDate = DateTime.Now;
                opinion.Id = User.FindFirstValue(ClaimTypes.NameIdentifier);
                _context.Add(opinion);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Texts", new {id=opinion.TextId}, "comments");
            }
            ViewData["IdText"] = opinion.TextId;
            ViewData["TextTitle"] = opinion.Text.Title;

            return View(opinion);
        }

        // POST: Opinions/Create
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePartial([Bind("OpinionId,Comment,Rating,TextId")] Opinion opinion)
        {
            if (ModelState.IsValid)
            {
                opinion.AddedDate = DateTime.Now;
                opinion.Id = User.FindFirstValue(ClaimTypes.NameIdentifier);
                _context.Add(opinion);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Texts", new { id = opinion.TextId }, "comments");
            }
            ViewData["IdText"] = opinion.TextId;
            ViewData["TextTitle"] = opinion.Text.Title;

            return RedirectToAction("Details", "Texts", new { id = opinion.TextId }, "comments");
        }

        // GET: Opinions/Edit/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Opinions == null)
            {
                return NotFound();
            }

            var opinion = await _context.Opinions.FindAsync(id);
            if (opinion == null)
            {
                return NotFound();
            }
            ViewData["TextId"] = opinion.TextId;
            ViewData["Author"] = opinion.Id;
            return View(opinion);
        }

        // POST: Opinions/Edit/5
        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int OpinionId, [Bind("OpinionId,Comment,Rating")] Opinion opinion)
        {
            if (OpinionId != opinion.OpinionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(opinion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OpinionExists(opinion.OpinionId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["TextId"] = opinion.TextId;
            ViewData["Author"] = opinion.Id;
            return View(opinion);
        }

        // GET: Opinions/Delete/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Opinions == null)
            {
                return NotFound();
            }

            var opinion = await _context.Opinions
                .Include(o => o.Text)
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.OpinionId == id);
            if (opinion == null)
            {
                return NotFound();
            }

            return View(opinion);
        }

        // POST: Opinions/Delete/5
        [Authorize(Roles = "admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Opinions == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Opinions'  is null.");
            }
            var opinion = await _context.Opinions.FindAsync(id);
            if (opinion != null)
            {
                _context.Opinions.Remove(opinion);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OpinionExists(int id)
        {
          return _context.Opinions.Any(e => e.OpinionId == id);
        }
    }
}
