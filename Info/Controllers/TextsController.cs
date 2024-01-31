using Info.Data;
using Info.Infrastructure;
using Info.Models;
using Info.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;
using Text = Info.Models.Text;

namespace Info.Controllers
{
    public class TextsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private IWebHostEnvironment _hostEnvironment;

        public TextsController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _hostEnvironment = environment;
        }

        // GET: Texts
        public async Task<IActionResult> Index(string Fraza, string Autor, int? Kategoria, int PageNumber = 1)
        {
            var SelectedTexts = _context.Texts?
                 .Include(t => t.Category)
                 .Include(t => t.User)
                 .Where(t => t.Active == true)
                 .OrderByDescending(t => t.AddedDate);

            if (Kategoria != null)
            {
                SelectedTexts = (IOrderedQueryable<Text>)SelectedTexts.Where(r => r.Category.CategoryId == Kategoria);
            }
            if (!String.IsNullOrEmpty(Autor))
            {
                SelectedTexts = (IOrderedQueryable<Text>)SelectedTexts.Where(r => r.User.Id == Autor);
            }
            if (!String.IsNullOrEmpty(Fraza))
            {
                SelectedTexts = (IOrderedQueryable<Text>)SelectedTexts.Where(r => r.Content.Contains(Fraza));
            }

            TextsViewModel textsViewModel = new();
            textsViewModel.TextsView = new TextsView();

            textsViewModel.TextsView.TextCount = SelectedTexts.Count();
            textsViewModel.TextsView.PageNumber = PageNumber;
            textsViewModel.TextsView.Author = Autor;
            textsViewModel.TextsView.Phrase = Fraza;
            textsViewModel.TextsView.Category = Kategoria;

            textsViewModel.Texts = (IEnumerable<Text>?)await SelectedTexts
                .Skip((PageNumber - 1) * textsViewModel.TextsView.PageSize) 
                .Take(textsViewModel.TextsView.PageSize)
                .ToListAsync();

            ViewData["Category"] = new SelectList(_context.Categories?
                .Where(c => c.Active == true),
                "CategoryId", "Name", Kategoria);

            ViewData["Author"] = new SelectList(_context.Texts
                .Include(u => u.User)
                .Select(u => u.User)
                .Distinct(),
                "Id", "FullName", Autor);

            return View(textsViewModel);
        }

        // GET: Texts
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> List()
        {
            var applicationDbContext = _context.Texts.Include(t => t.Category).Include(t => t.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Texts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Texts == null)
            {
                return NotFound();
            }

            TextWithOpinions textWithOpinions = new TextWithOpinions();

            textWithOpinions.SelectedText = await _context.Texts
                .Include(t => t.Category)
                .Include(t => t.User)
                .Include(t => t.Opinions)
                .ThenInclude(c => c.User)
                .Where(t => t.Active == true)
                .FirstOrDefaultAsync(m => m.TextId == id);
               
            if (textWithOpinions.SelectedText == null)
            {
                return NotFound();
            }
            textWithOpinions.NewOpinion = new Opinion { TextId = (int)id, 
                                    Id=textWithOpinions.SelectedText.Id };

            textWithOpinions.ReadingTime = (int)Math.Ceiling((double)textWithOpinions.SelectedText.Content.Length / 1400);

            textWithOpinions.CommentsNumber = _context.Opinions
                .Where(x => x.TextId == id)
                .Count();

            if (textWithOpinions.CommentsNumber != 0)
            {
                textWithOpinions.OpinionsNumber = _context.Opinions.Where(o => o.TextId == id).Where(x => x.Rating != null).Count();
                if(textWithOpinions.OpinionsNumber != 0)
                {
                    textWithOpinions.AverageScore = (float)(textWithOpinions.OpinionsNumber > 0 ? _context.Opinions.Where(o => o.TextId == id).Where(x => x.Rating != null).Average(x => (int)x.Rating) : 0);
                }
            }

            textWithOpinions.Description = Variaty.Phrase("komentarz", "komentarze", "komentarzy", textWithOpinions.CommentsNumber);
            

            return View(textWithOpinions);
        }

        // GET: Texts/Create
        [Authorize(Roles = "admin, author")]
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name");
            return View();
        }

        // POST: Texts/Create
        [Authorize(Roles = "admin, author")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TextId,Title,Summary,Keywords,Content,Active,CategoryId")] Info.Models.Text text, IFormFile picture)
        {
            if (ModelState.IsValid)
            {
                //odczytanie identyfikatora użytkownika i bieżącej daty
                text.Id = User.FindFirstValue(ClaimTypes.NameIdentifier);
                text.AddedDate = DateTime.Now;

                //zapisanie obrazka i przeskalowanej wersji
                if (picture != null && picture.Length > 0)
                {
                    ImageFileUpload imageFileResult = new(_hostEnvironment);
                    FileSendResult fileSendResult = imageFileResult.SendFile(picture, "img", 600);
                    if (fileSendResult.Success)
                    {
                        text.Graphic = fileSendResult.Name;
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Wybrany plik nie jest obrazkiem!";
                        ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name", text.CategoryId);
                        return View(text);
                    }
                }

                _context.Add(text);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name", text.CategoryId);
            return View(text);
        }

        // GET: Texts/Edit/5
        [Authorize(Roles = "admin, author")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Texts == null)
            {
                return NotFound();
            }

            var text = await _context.Texts.FindAsync(id);
            if (text == null)
            {
                return NotFound();
            }

            if(string.Compare(User.FindFirstValue(ClaimTypes.NameIdentifier), text.Id) == 0 || User.IsInRole("admin"))
            {
                ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name", text.CategoryId);
                ViewData["Author"] = text.Id;
                return View(text);
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Texts/Edit/5
        [Authorize(Roles = "admin, author")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int textid, [Bind("TextId,Title,Summary,Keywords,Content,Graphic,Active,AddedDate,CategoryId,Id")] Info.Models.Text text, IFormFile? picture)
        {
            if (textid != text.TextId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {

                //zapisanie obrazka i przeskalowanej kopii
                if (picture != null && picture.Length > 0)
                {
                    ImageFileUpload imageFileResult = new(_hostEnvironment);
                    FileSendResult fileSendResult = imageFileResult.SendFile(picture, "img", 600);
                    if (fileSendResult.Success)
                    {
                        text.Graphic = fileSendResult.Name;
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Wybrany plik nie jest obrazkiem!";
                        ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name", text.CategoryId);
                        ViewData["Author"] = text.Id;
                        return View(text);
                    }
                }

                try
                {
                    _context.Update(text);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TextExists(text.TextId))
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name", text.CategoryId);
            ViewData["Author"] = text.Id;
            return View(text);
        }

        // GET: Texts/Delete/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Texts == null)
            {
                return NotFound();
            }

            var text = await _context.Texts
                .Include(t => t.Category)
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.TextId == id);
            if (text == null)
            {
                return NotFound();
            }

            return View(text);
        }

        // POST: Texts/Delete/5
        [Authorize(Roles = "admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Texts == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Texts'  is null.");
            }
            var text = await _context.Texts.FindAsync(id);
            if (text != null)
            {
                _context.Texts.Remove(text);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TextExists(int id)
        {
          return _context.Texts.Any(e => e.TextId == id);
        }
    }
}
