using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MYMVCAPP.Models;
using MYMVCAPP.Repository;
using System.Linq;
using System.Threading.Tasks;

namespace MYMVCAPP.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly DataContext _dataContext;

        public CategoryController(DataContext context)
        {
            _dataContext = context;
        }

        // GET: Admin/Category
        public async Task<IActionResult> Index()
        {
            var categories = await _dataContext.Categories
                .OrderByDescending(c => c.Id)
                .ToListAsync();
            return View(categories);
        }

       // GET: Admin/Category/Create
        public IActionResult Create()
        {
            ViewBag.Menus = new SelectList(_dataContext.Menus, "Id", "Name");
            return View();
        }

        // POST: Admin/Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryModel category)
        {
            if (ModelState.IsValid)
            {
                category.Slug = category.Name.Replace(" ", "-");

                var exists = await _dataContext.Categories
                    .AnyAsync(c => c.Slug == category.Slug);
                if (exists)
                {
                    ModelState.AddModelError("", "Danh mục đã tồn tại!");
                    ViewBag.Menus = new SelectList(_dataContext.Menus, "Id", "Name", category.MenuId);
                    return View(category);
                }

                _dataContext.Categories.Add(category);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Thêm danh mục thành công!";
                return RedirectToAction("Index");
            }

            ViewBag.Menus = new SelectList(_dataContext.Menus, "Id", "Name", category.MenuId);
            return View(category);
        }

        // GET: Admin/Category/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _dataContext.Categories.FindAsync(id);
            if (category == null) return NotFound();

            ViewBag.Menus = new SelectList(_dataContext.Menus, "Id", "Name", category.MenuId);
            return View(category);
        }

        // POST: Admin/Category/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryModel category)
        {
            if (id != category.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                category.Slug = category.Name.Replace(" ", "-");
                _dataContext.Categories.Update(category);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Cập nhật danh mục thành công!";
                return RedirectToAction("Index");
            }

            ViewBag.Menus = new SelectList(_dataContext.Menus, "Id", "Name", category.MenuId);
            return View(category);
        }


        // GET: Admin/Category/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _dataContext.Categories.FindAsync(id);
            if (category == null) return NotFound();

            _dataContext.Categories.Remove(category);
            await _dataContext.SaveChangesAsync();
            TempData["success"] = "Xóa danh mục thành công!";
            return RedirectToAction("Index");
        }
    }
}
