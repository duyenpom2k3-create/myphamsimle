using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MYMVCAPP.Repository;
using MYMVCAPP.Models;
using System.Threading.Tasks;
using System.Linq;

namespace MYMVCAPP.Controllers
{
    public class BrandController : Controller
    {
        private readonly DataContext _dataContext;

        public BrandController(DataContext context)
        {
            _dataContext = context;
        }

        // ✅ Hiển thị danh sách thương hiệu trên trang Home (partial)
        public async Task<IActionResult> ListPartial()
        {
            var brands = await _dataContext.Brands
                .OrderBy(b => b.Name)
                .Take(12)
                .ToListAsync();

            return PartialView("~/Views/Shared/Brand/Default.cshtml", brands);
        }

        // ✅ Trang hiển thị tất cả thương hiệu (Brand/Index)
        public async Task<IActionResult> Index()
        {
            var brands = await _dataContext.Brands
                .OrderBy(b => b.Name)
                .ToListAsync();

            return View(brands);
        }

        // ✅ Trang sản phẩm theo thương hiệu
        //    VD: /Brand/cocoon → hiển thị sản phẩm thương hiệu Cocoon
        [HttpGet("Brand/{slug}")]
        public async Task<IActionResult> Products(string slug)
        {
            // Kiểm tra slug có hợp lệ không
            if (string.IsNullOrWhiteSpace(slug))
                return NotFound();

            // Tìm thương hiệu theo slug
            var brand = await _dataContext.Brands
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Slug == slug);

            if (brand == null)
                return NotFound();

            // Lấy danh sách sản phẩm thuộc thương hiệu đó
            var products = await _dataContext.Products
                .AsNoTracking()
                .Where(p => p.BrandId == brand.Id)
                .ToListAsync();

            // Truyền brand qua ViewBag để hiển thị tên và ảnh logo
            ViewBag.Brand = brand;

            // Render view Products.cshtml (Views/Brand/Products.cshtml)
            return View("Products", products);
        }
    }
}
