using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MYMVCAPP.Models;
using MYMVCAPP.Repository;
using System.Linq;
using System.Threading.Tasks;

namespace MYMVCAPP.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]/[action]/{id?}")] // ✅ Giúp MVC định tuyến chính xác, tránh trùng action
    public class FooterItemController : Controller
    {
        private readonly DataContext _dataContext;

        public FooterItemController(DataContext context)
        {
            _dataContext = context;
        }

        // ====================== INDEX ======================
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var footer = await _dataContext.FooterItems
                .OrderByDescending(m => m.Id)
                .ToListAsync();
            return View(footer);
        }

        // ====================== CREATE ======================
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FooterItem footer)
        {
            if (!ModelState.IsValid)
                return View(footer);

            footer.Slug = GenerateSlug(footer.ItemTitle ?? "");

            // Kiểm tra trùng Slug
            var exists = await _dataContext.FooterItems.AnyAsync(m => m.Slug == footer.Slug);
            if (exists)
            {
                ModelState.AddModelError("", "Footer đã tồn tại!");
                return View(footer);
            }

            _dataContext.FooterItems.Add(footer);
            await _dataContext.SaveChangesAsync();

            TempData["success"] = "✅ Thêm footer thành công!";
            return RedirectToAction(nameof(Index));
        }

        // ====================== EDIT ======================
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var footer = await _dataContext.FooterItems.FindAsync(id);
            if (footer == null) return NotFound();

            return View(footer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FooterItem footer)
        {
            if (id != footer.Id) return BadRequest();

            if (!ModelState.IsValid)
                return View(footer);

            try
            {
                footer.Slug = GenerateSlug(footer.ItemTitle ?? "");
                _dataContext.FooterItems.Update(footer);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "✅ Cập nhật footer thành công!";
            }
            catch (DbUpdateException)
            {
                TempData["error"] = "❌ Lỗi khi cập nhật dữ liệu!";
            }

            return RedirectToAction(nameof(Index));
        }

        // ====================== DELETE ======================
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var footer = await _dataContext.FooterItems.FindAsync(id);
            if (footer == null) return NotFound();

            _dataContext.FooterItems.Remove(footer);
            await _dataContext.SaveChangesAsync();

            TempData["success"] = "🗑️ Xóa footer thành công!";
            return RedirectToAction(nameof(Index));
        }

        // ====================== HÀM SINH SLUG ======================
        private string GenerateSlug(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "";
            return name.Trim().Replace(" ", "-").ToLower();
        }
    }
}
