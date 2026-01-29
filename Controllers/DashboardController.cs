using LibraryManagement.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        public DashboardController(ApplicationDbContext context)
        {
           _context = context;
        }
        // GET: DashboardController
// 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
        public async Task<IActionResult> Index()
        {
            var totalBooks = await _context.Books.CountAsync();
            ViewData["TotalBooks"] = totalBooks;
            var availableBooks = await _context.Books.CountAsync(p => p.IsAvailable == true);
            ViewData["AvailableBooks"] = availableBooks;
            var borrowedBooks = await _context.BorrowRecords.CountAsync(p => p.ReturnDate == null);
            ViewData["BorrowedBooks"] = borrowedBooks;
            var totalMembers = await _context.Members.CountAsync();
            ViewData["TotalMembers"] = totalMembers;
            var twoWeeksAgo = DateTime.Now.AddDays(-14);
            var overDueBooks = await _context.BorrowRecords.CountAsync(pr => pr.BorrowDate < twoWeeksAgo && pr.ReturnDate == null);
            ViewData["OverDueBooks"] = overDueBooks;
            var today = DateTime.Today;
            var todaysBorrow = await _context.BorrowRecords.CountAsync(pr => pr.BorrowDate.Date == today);
            ViewData["TodaysBorrow"] = todaysBorrow;
            var recentBorrows = await _context.BorrowRecords.Include(p => p.Book).Include(p => p.Member).OrderByDescending(p => p.BorrowDate).Take(5).ToListAsync();
            ViewData["RecentBorrows"] = recentBorrows;
            return View();
        }

    }
}
