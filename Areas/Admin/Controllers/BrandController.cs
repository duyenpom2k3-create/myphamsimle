using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MYMVCAPP.Models;
using MYMVCAPP.Repository;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MYMVCAPP.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BrandController : Controller
    {
        private readonly DataContext _dataContext;

        public BrandController(DataContext context)
        {
            _dataContext = context;
        }

        // ================= Index =================
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var brands = await _dataContext.Brands
                .OrderByDescending(b => b.Id)
                .ToListAsync();
            return View(brands);
        }

        // ================= Create =================
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BrandModel brand, IFormFile LogoFile)
        {
            if (!ModelState.IsValid)
                return View(brand);

            brand.Slug = brand.Name.Trim().Replace(" ", "-");

            if (await _dataContext.Brands.AnyAsync(b => b.Slug == brand.Slug))
            {
                ModelState.AddModelError("", "Thương hiệu đã tồn tại!");
                return View(brand);
            }

            // Upload logo
            if (LogoFile != null && LogoFile.Length > 0)
            {
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/brand");
                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                var fileName = $"{DateTime.Now.Ticks}_{Path.GetFileName(LogoFile.FileName)}";
                var filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await LogoFile.CopyToAsync(stream);
                }

                brand.Image = fileName;
            }

            _dataContext.Brands.Add(brand);
            await _dataContext.SaveChangesAsync();
            TempData["success"] = "Thêm thương hiệu thành công!";
            return RedirectToAction(nameof(Index));
        }

        // ================= Edit =================
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var brand = await _dataContext.Brands.FindAsync(id);
            if (brand == null) return NotFound();
            return View(brand);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BrandModel model, IFormFile LogoFile)
        {
            if (id != model.Id)
                return BadRequest();

            var brand = await _dataContext.Brands.FirstOrDefaultAsync(b => b.Id == id);
            if (brand == null) return NotFound();

            if (!ModelState.IsValid)
                return View(model);

            brand.Name = model.Name;
            brand.Description = model.Description;
            brand.Status = model.Status;
            brand.Slug = model.Name.Trim().Replace(" ", "-");

            if (LogoFile != null && LogoFile.Length > 0)
            {
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/brand");
                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                var fileName = $"{DateTime.Now.Ticks}_{Path.GetFileName(LogoFile.FileName)}";
                var filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await LogoFile.CopyToAsync(stream);
                }

                brand.Image = fileName;
            }

            try
            {
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Cập nhật thương hiệu thành công!";
            }
            catch (DbUpdateException)
            {
                TempData["error"] = "Lỗi khi cập nhật dữ liệu!";
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        // ================= Delete =================
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var brand = await _dataContext.Brands.FindAsync(id);
            if (brand == null) return NotFound();

            _dataContext.Brands.Remove(brand);
            await _dataContext.SaveChangesAsync();
            TempData["success"] = "Xóa thương hiệu thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}
