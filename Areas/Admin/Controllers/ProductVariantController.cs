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
    public class ProductVariantController : Controller
    {
        private readonly DataContext _dataContext;

        public ProductVariantController(DataContext context)
        {
            _dataContext = context;
        }

        // GET: Admin/ProductVariant
        public async Task<IActionResult> Index()
        {
            var variants = await _dataContext.ProductVariants
                .Include(v => v.Product)
                .OrderByDescending(v => v.Id)
                .ToListAsync();
            return View(variants);
        }

        // GET: Admin/ProductVariant/Create
        public async Task<IActionResult> Create()
        {
            ViewData["ProductId"] = new SelectList(await _dataContext.Products.ToListAsync(), "Id", "Name");
            return View();
        }

        // POST: Admin/ProductVariant/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductVariant variant)
        {
            if (!ModelState.IsValid)
            {
                ViewData["ProductId"] = new SelectList(await _dataContext.Products.ToListAsync(), "Id", "Name", variant.ProductId);
                return View(variant);
            }

            _dataContext.ProductVariants.Add(variant);
            await _dataContext.SaveChangesAsync();
            TempData["success"] = "Thêm biến thể thành công!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/ProductVariant/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var variant = await _dataContext.ProductVariants.FindAsync(id);
            if (variant == null) return NotFound();

            ViewData["ProductId"] = new SelectList(await _dataContext.Products.ToListAsync(), "Id", "Name", variant.ProductId);
            return View(variant);
        }

        // POST: Admin/ProductVariant/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductVariant variant)
        {
            if (id != variant.Id) return BadRequest();

            if (!ModelState.IsValid)
            {
                ViewData["ProductId"] = new SelectList(await _dataContext.Products.ToListAsync(), "Id", "Name", variant.ProductId);
                return View(variant);
            }

            try
            {
                _dataContext.ProductVariants.Update(variant);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Cập nhật biến thể thành công!";
            }
            catch (DbUpdateException)
            {
                TempData["error"] = "Lỗi khi cập nhật dữ liệu!";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/ProductVariant/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var variant = await _dataContext.ProductVariants
                .Include(v => v.Product)
                .FirstOrDefaultAsync(v => v.Id == id);
            if (variant == null) return NotFound();

            _dataContext.ProductVariants.Remove(variant);
            await _dataContext.SaveChangesAsync();
            TempData["success"] = "Xóa biến thể thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}
