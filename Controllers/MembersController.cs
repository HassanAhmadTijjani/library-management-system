using LibraryManagement.Data;
using LibraryManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Controllers
{
    public class MembersController : Controller
    {
        private readonly ApplicationDbContext _context;
        public MembersController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: MembersController
        // public ActionResult Index()
        // {
        //     return View();
        // }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(string searchString)
        {
            var members = _context.Members.AsQueryable();
            if (!string.IsNullOrEmpty(searchString))
            {
                var toLower = searchString.ToLower();
                members = members.Where(p => p.Name.ToLower().Contains(toLower));
            }
            ViewData["CurrentFilter"] = searchString;
            return View(await members.ToListAsync());
        }

        // Get: Details/Read-One-OBJ
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var member = await _context.Members.FirstOrDefaultAsync(p => p.Id == id);
            if (member == null)
            {
                return NotFound();
            }
            return View(member);
        }

        // Get: Create/Member/Upsert
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Upsert(int? id)
        {
            if (id == null || id == 0)
            {
                return View(new Member());
            }
            else
            {
                var member = await _context.Members.FindAsync(id);
                if (member == null)
                {
                    return NotFound();
                }
                return View(member);
            }
        }
        // Post: Create/Member/Upsert
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert([Bind("Id,Name,Email,MemberShipDate")] Member member)
        {
            if (ModelState.IsValid)
            {
                if (member.Id == 0)
                {
                    _context.Add(member);
                    TempData["success"] = "Member added successfully";
                }
                else
                {
                    _context.Update(member);
                    TempData["success"] = "Member updated successfully";
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(member);
        }

        // Get: Edit/Member
        // public async Task<IActionResult> Edit(int? id)
        // {
        //     if (id == null || id <= 0)
        //     {
        //         return NotFound();
        //     }
        //     var member = await _context.Members.FindAsync(id);
        //     if (member == null)
        //     {
        //         return NotFound();
        //     }
        //     return View(member);
        // }
        // [HttpPost]
        // public async Task<IActionResult> Edit(int id, [Bind("Id, Name, Email, MemberShipDate")] Member member)
        // {
        //     if (id != member.Id)
        //     {
        //         return NotFound();
        //     }
        //     if (ModelState.IsValid)
        //     {
        //         try
        //         {
        //             _context.Update(member);
        //             await _context.SaveChangesAsync();
        //             TempData["success"] = "Member updated successfully!";
        //         }
        //         catch (DbUpdateConcurrencyException)
        //         {
        //             if (!MemberExists(member.Id))
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
        //     return View(member);
        // }

        // Get: Delete/Member
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member = await _context.Members
                .FirstOrDefaultAsync(m => m.Id == id);

            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        // Post: Delete/Member
        [Authorize(Roles = "Admin")]

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var member = await _context.Members.FindAsync(id);
            if (member != null)
            {
                _context.Members.Remove(member);
            }
            await _context.SaveChangesAsync();
            TempData["success"] = "Member deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        private bool MemberExists(int id)
        {
            return _context.Members.Any(e => e.Id == id);
        }
    }
}
