using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MYMVCAPP.Models;
using MYMVCAPP.Models.ViewModels;
using MYMVCAPP.Repository;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace MYMVCAPP.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly DataContext _dataContext;

        public ProductController(DataContext context)
        {
            _dataContext = context;
        }

        // ================= Index =================
        public async Task<IActionResult> Index()
        {
            var products = await _dataContext.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .OrderByDescending(p => p.Id)
                .ToListAsync();
            return View(products);
        }

        // ================= Create =================
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name");
            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductModel product, IFormFile ProductImage)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name", product.BrandId);
                ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", product.CategoryId);
                return View(product);
            }

            product.Slug = GenerateSlug(product.Name);

            if (await _dataContext.Products.AnyAsync(p => p.Slug == product.Slug))
            {
                ModelState.AddModelError("", "Sản phẩm đã tồn tại!");
                ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name", product.BrandId);
                ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", product.CategoryId);
                return View(product);
            }

            // Upload ảnh
            if (ProductImage != null && ProductImage.Length > 0)
            {
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products");
                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                var fileName = $"{DateTime.Now.Ticks}_{Path.GetFileName(ProductImage.FileName)}";
                var filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ProductImage.CopyToAsync(stream);
                }

                product.Img = fileName;
            }

            _dataContext.Products.Add(product);
            await _dataContext.SaveChangesAsync();
            TempData["success"] = "Thêm sản phẩm thành công!";
            return RedirectToAction(nameof(Index));
        }

        // ================= Edit =================
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _dataContext.Products.FindAsync(id);
            if (product == null) return NotFound();

            ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name", product.BrandId);
            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductModel model, IFormFile ProductImage)
        {
            if (id != model.Id) return BadRequest();

            var product = await _dataContext.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name", model.BrandId);
                ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", model.CategoryId);
                return View(model);
            }

            var newSlug = GenerateSlug(model.Name);
            if (await _dataContext.Products.AnyAsync(p => p.Slug == newSlug && p.Id != id))
            {
                ModelState.AddModelError("", "Tên sản phẩm trùng với sản phẩm khác!");
                ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name", model.BrandId);
                ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", model.CategoryId);
                return View(model);
            }

            // Cập nhật thông tin sản phẩm
            product.Name = model.Name;
            product.Description = model.Description;
            product.Price = model.Price;
            product.BrandId = model.BrandId;
            product.CategoryId = model.CategoryId;
            product.Slug = newSlug;

            // Upload ảnh mới nếu có
            if (ProductImage != null && ProductImage.Length > 0)
            {
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products");
                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                var fileName = $"{DateTime.Now.Ticks}_{Path.GetFileName(ProductImage.FileName)}";
                var filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ProductImage.CopyToAsync(stream);
                }

                product.Img = fileName;
            }

            try
            {
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Cập nhật sản phẩm thành công!";
            }
            catch (DbUpdateException)
            {
                TempData["error"] = "Có lỗi khi cập nhật dữ liệu!";
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        // ================= Delete =================
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _dataContext.Products.FindAsync(id);
            if (product == null) return NotFound();

            _dataContext.Products.Remove(product);
            await _dataContext.SaveChangesAsync();
            TempData["success"] = "Xóa sản phẩm thành công!";
            return RedirectToAction(nameof(Index));
        }

        // ================= Search =================
        public IActionResult Search(string keyword)
        {
            var result = new List<ProductModel>();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                result = _dataContext.Products
                    .Where(p => p.Name.Contains(keyword) || p.Description.Contains(keyword))
                    .ToList();
            }

            ViewBag.Keyword = keyword;
            return View("Search", result);
        }

        // ================= GenerateSlug =================
        private string GenerateSlug(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "";
            return name.Trim().Replace(" ", "-").ToLower();
        }
    }
}
