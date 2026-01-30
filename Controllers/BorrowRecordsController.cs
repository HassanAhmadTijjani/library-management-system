using LibraryManagement.Data;
using LibraryManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Controllers
{
    public class BorrowRecordsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public BorrowRecordsController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: BorrowRecordsController
        // public ActionResult Index()
        // {
        //     return View();
        // }
        public async Task<IActionResult> Index()
        {
            var borrowRecords = await _context.BorrowRecords.Include(p => p.Book).Include(p => p.Member).ToListAsync();
            return View(borrowRecords);
        }
        // Get: BorrowRecords By ID
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var borrowRecord = await _context.BorrowRecords.Include(p => p.Book).Include(p => p.Member).FirstOrDefaultAsync(p => p.Id == id);
            if (borrowRecord == null)
            {
                return NotFound();
            }
            return View(borrowRecord);
        }

        // Get: Create/BorrowREcords
        public async Task<IActionResult> Upsert(int? id)
        {
            ViewData["BookId"] = new SelectList(_context.Books.Where(p => p.IsAvailable), "Id", "Title");
            ViewData["MemberId"] = new SelectList(_context.Members, "Id", "Name");
            if (id == null || id == 0)
            {
                return View(new BorrowRecord());
            }
            else
            {
                var borrowRecord = await _context.BorrowRecords.FindAsync(id);
                if (borrowRecord == null)
                {
                    return NotFound();
                }
                return View(borrowRecord);
            }
        }

        // Post: BorrowRecords/Create/Upsert
        // POST: BorrowRecords/Create/Upsert
        [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Upsert([Bind("Id,BookId,MemberId,BorrowDate,ReturnDate")] BorrowRecord borrowRecord)
{
    ModelState.Remove("Book");
    ModelState.Remove("Member");

    if (ModelState.IsValid)
    {
        if (borrowRecord.Id == 0)
        {
            // Creating new borrow record
            var book = await _context.Books.FindAsync(borrowRecord.BookId);
            if (book != null)
            {
                book.IsAvailable = false;
            }
            
            _context.Add(borrowRecord);
            TempData["success"] = "Book borrowed successfully!";
        }
        else
        {
            // Updating existing record
            var existingRecord = await _context.BorrowRecords
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == borrowRecord.Id);
            
            // If return date is set and it was null before, mark book as available
            if (borrowRecord.ReturnDate != null && existingRecord?.ReturnDate == null)
            {
                var book = await _context.Books.FindAsync(borrowRecord.BookId);
                if (book != null)
                {
                    book.IsAvailable = true;
                }
            }
            
            _context.Update(borrowRecord);
            TempData["success"] = "Borrow record updated successfully!";
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    ViewData["BookId"] = new SelectList(_context.Books.Where(b => b.IsAvailable), "Id", "Title", borrowRecord.BookId);
    ViewData["MemberId"] = new SelectList(_context.Members, "Id", "Name", borrowRecord.MemberId);
    return View(borrowRecord);
}
        // Get: BorrowRecord/Edit
        // public async Task<IActionResult> Edit(int? id)
        // {
        //     if (id == null)
        //     {
        //         return NotFound();
        //     }

        //     var borrowRecord = await _context.BorrowRecords.FindAsync(id);
        //     if (borrowRecord == null)
        //     {
        //         return NotFound();
        //     }

        //     ViewData["BookId"] = new SelectList(_context.Books, "Id", "Title", borrowRecord.BookId);
        //     ViewData["MemberId"] = new SelectList(_context.Members, "Id", "Name", borrowRecord.MemberId);
        //     return View(borrowRecord);
        // }

        // Post: BorrowRecord/Edit
        // POST: BorrowRecords/Edit/5
        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> Edit(int id, [Bind("Id,BookId,MemberId,BorrowDate,ReturnDate")] BorrowRecord borrowRecord)
        // {
        //     if (id != borrowRecord.Id)
        //     {
        //         return NotFound();
        //     }

        //     // TEMPORARY: Remove validation errors for navigation properties
        //     ModelState.Remove("Book");
        //     ModelState.Remove("Member");

        //     if (ModelState.IsValid)
        //     {
        //         try
        //         {
        //             var existingRecord = await _context.BorrowRecords.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id);

        //             // If return date is set and it was null before, mark book as available
        //             if (borrowRecord.ReturnDate != null && existingRecord?.ReturnDate == null)
        //             {
        //                 var book = await _context.Books.FindAsync(borrowRecord.BookId);
        //                 if (book != null)
        //                 {
        //                     book.IsAvailable = true;
        //                 }
        //             }

        //             _context.Update(borrowRecord);
        //             await _context.SaveChangesAsync();
        //             TempData["success"] = "Borrow record updated successfully!";
        //         }
        //         catch (DbUpdateConcurrencyException)
        //         {
        //             if (!BorrowRecordExists(borrowRecord.Id))
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

        //     ViewData["BookId"] = new SelectList(_context.Books, "Id", "Title", borrowRecord.BookId);
        //     ViewData["MemberId"] = new SelectList(_context.Members, "Id", "Name", borrowRecord.MemberId);
        //     return View(borrowRecord);
        // }



        // Get: Delete/BorrowRecord
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var borrowRecord = await _context.BorrowRecords
                .Include(b => b.Book)
                .Include(b => b.Member)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (borrowRecord == null)
            {
                return NotFound();
            }

            return View(borrowRecord);
        }

        // Post: Delete/BorrowRecord
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var borrowRecord = await _context.BorrowRecords.FindAsync(id);
            if (borrowRecord != null)
            {
                // If book was not returned, mark it as available again
                if (borrowRecord.ReturnDate == null)
                {
                    var book = await _context.Books.FindAsync(borrowRecord.BookId);
                    if (book != null)
                    {
                        book.IsAvailable = true;
                    }
                }

                _context.BorrowRecords.Remove(borrowRecord);
            }

            await _context.SaveChangesAsync();
            TempData["success"] = "Borrow record deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        private bool BorrowRecordExists(int id)
        {
            return _context.BorrowRecords.Any(e => e.Id == id);
        }

    }
}
