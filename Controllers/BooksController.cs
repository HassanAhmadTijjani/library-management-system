using System;
using System.Linq;
using LibraryManagement.Data;
using LibraryManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Controllers
{
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _context;
        public BooksController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: BooksController
        // public ActionResult Index()
        // {
        //     return View();
        // }
        [Authorize]
        public async Task<IActionResult> Index(string searchString)
        {
            var books = _context.Books.AsQueryable();
            if (!string.IsNullOrEmpty(searchString))
            {
                var toLower = searchString.ToLower();
                books = books.Where(p => p.Title.ToLower().Contains(toLower) || p.Author.ToLower().Contains(toLower));
            }
            ViewData["CurrentFilter"] = searchString;
            return View(await books.ToListAsync());
        }

        // GET: Books/Details/Id
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var book = await _context.Books.FirstOrDefaultAsync(p => p.Id == id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        // Get: Create/Books/Upsert
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Upsert(int? id)
        {

            if (id == null || id == 0)
            {
                return View(new Book());
            }
            else
            {
                var book = await _context.Books.FindAsync(id);
                if (book == null)
                {
                    return NotFound();
                }
                return View(book);
            }
        }

        // POST: Books/Create/Upsert
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(Book book, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                // Handle image upload
                if (imageFile != null && imageFile.Length > 0)
                {
                    // Validate file size (2MB max)
                    if (imageFile.Length > 2 * 1024 * 1024)
                    {
                        ModelState.AddModelError("imageFile", "Image size cannot exceed 2MB");
                        return View(book);
                    }

                    // Validate file type
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var extension = Path.GetExtension(imageFile.FileName).ToLower();
                    if (!allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("imageFile", "Only JPG, PNG, and GIF images are allowed");
                        return View(book);
                    }

                    // Delete old image if updating
                    if (book.Id != 0 && !string.IsNullOrEmpty(book.ImageUrl))
                    {
                        var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", book.ImageUrl);
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    // Generate unique filename
                    var fileName = Guid.NewGuid().ToString() + extension;
                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "books");

                    // Ensure directory exists
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    var filePath = Path.Combine(uploadPath, fileName);

                    // Save file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    // Store relative path in database
                    book.ImageUrl = Path.Combine("images", "books", fileName).Replace("\\", "/");
                }

                if (book.Id == 0)
                {
                    _context.Add(book);
                    TempData["success"] = "Book created successfully!";
                }
                else
                {
                    _context.Update(book);
                    TempData["success"] = "Book updated successfully!";
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(book);
        }
        // Get: Books/Edit
        // public async Task<IActionResult> Edit(int? id)
        // {
        //     if (id == null || id <= 0)
        //     {
        //         return NotFound();
        //     }
        //     var book = await _context.Books.FindAsync(id);
        //     if (book == null)
        //     {
        //         return NotFound();
        //     }
        //     return View(book);
        // }

        // Post: Book/Edit
        // [HttpPost]
        // public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Author,ISBN,PublishedYear,IsAvailable")] Book book)
        // {
        //     if (id != book.Id)
        //     {
        //         return NotFound();
        //     }
        //     if (ModelState.IsValid)
        //     {
        //         try
        //         {
        //             await _context.SaveChangesAsync();
        //         }
        //         catch (DbUpdateConcurrencyException)
        //         {
        //             if (!BookExists(book.Id))
        //             {
        //                 return NotFound();
        //             }
        //             else
        //             {
        //                 throw;
        //             }
        //         }
        //         return RedirectToAction(nameof(Index));
        //     }
        //     return View(book);
        // }

        // Get: Delete
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .FirstOrDefaultAsync(m => m.Id == id);

            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // Post: Delete
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
            }

            await _context.SaveChangesAsync();
            TempData["success"] = "Book deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }
    }

}
