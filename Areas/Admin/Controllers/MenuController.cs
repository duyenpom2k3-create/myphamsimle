using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MYMVCAPP.Models;
using MYMVCAPP.Repository;
using System.Threading.Tasks;
using System.Linq;

namespace MYMVCAPP.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MenuController : Controller
    {
        private readonly DataContext _dataContext;

        public MenuController(DataContext context)
        {
            _dataContext = context;
        }

        // GET: Admin/Menu
        public async Task<IActionResult> Index()
        {
            var menus = await _dataContext.Menus
                .OrderByDescending(m => m.Id)
                .ToListAsync();
            return View(menus);
        }

        // GET: Admin/Menu/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Menu/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MenuModel menu)
        {
            if (!ModelState.IsValid)
                return View(menu);

            menu.Slug = GenerateSlug(menu.Name);

            // Kiểm tra trùng Slug
            var exists = await _dataContext.Menus
                .AnyAsync(m => m.Slug == menu.Slug);
            if (exists)
            {
                ModelState.AddModelError("", "Menu đã tồn tại!");
                return View(menu);
            }

            _dataContext.Menus.Add(menu);
            await _dataContext.SaveChangesAsync();
            TempData["success"] = "Thêm menu thành công!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/Menu/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var menu = await _dataContext.Menus.FindAsync(id);
            if (menu == null) return NotFound();
            return View(menu);
        }

        // POST: Admin/Menu/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MenuModel menu)
        {
            if (id != menu.Id) return BadRequest();

            if (!ModelState.IsValid) return View(menu);

            try
            {
                menu.Slug = GenerateSlug(menu.Name);
                _dataContext.Menus.Update(menu);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Cập nhật menu thành công!";
            }
            catch (DbUpdateException)
            {
                TempData["error"] = "Lỗi khi cập nhật dữ liệu!";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/Menu/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var menu = await _dataContext.Menus.FindAsync(id);
            if (menu == null) return NotFound();

            _dataContext.Menus.Remove(menu);
            await _dataContext.SaveChangesAsync();
            TempData["success"] = "Xóa menu thành công!";
            return RedirectToAction(nameof(Index));
        }

        // Hàm tiện ích tạo Slug
        private string GenerateSlug(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "";
            return name.Trim().Replace(" ", "-").ToLower();
        }
    }
}
