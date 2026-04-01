using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MYMVCAPP.Models;
using MYMVCAPP.Repository;
using System.Linq;
using System.Threading.Tasks;

namespace MYMVCAPP.Controllers
{
    public class ProductVariantController : Controller
    {
        private readonly DataContext _dataContext;

        public ProductVariantController(DataContext context)
        {
            _dataContext = context;
        }

        // GET: /ProductVariants/Index?productId=5
        public async Task<IActionResult> Index(int productId)
        {
            // Lấy sản phẩm
            var product = await _dataContext.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null) return NotFound();

            // Lấy danh sách biến thể
            var variants = await _dataContext.ProductVariants
                .Where(v => v.ProductId == productId)
                .ToListAsync();

            // Xác định có Weight/Color hay không
            ViewBag.HasWeight = variants.Any(v => !string.IsNullOrEmpty(v.Weight));
            ViewBag.HasColor = variants.Any(v => !string.IsNullOrEmpty(v.Color));
            ViewBag.Product = product;

            return View(variants);
        }
    }
}
