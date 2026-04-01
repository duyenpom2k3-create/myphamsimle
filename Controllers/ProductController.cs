using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MYMVCAPP.Repository;
using MYMVCAPP.Models;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace MYMVCAPP.Controllers
{
    public class ProductController : Controller
    {
        private readonly DataContext _dataContext;

        public ProductController(DataContext context)
        {
            _dataContext = context;
        }

        // ✅ Hiển thị danh sách tất cả sản phẩm
        public async Task<IActionResult> Index()
        {
            var products = await _dataContext.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .ToListAsync();
            return View(products);
        }

        // ✅ Thêm biến thể sản phẩm
        [HttpPost]
        public async Task<IActionResult> AddVariant(int productId, string color, string weight, decimal price, int stock)
        {
            var variant = new ProductVariant
            {
                ProductId = productId,
                Color = color,
                Weight = weight,
                Price = price,
                Stock = stock
            };

            _dataContext.ProductVariants.Add(variant);
            await _dataContext.SaveChangesAsync();

            return RedirectToAction("Edit", new { id = productId });
        }

        // ✅ Chi tiết sản phẩm - hỗ trợ song song cả Id và Slug
        [HttpGet]
        [Route("Product/Details/{id:int?}")]
        [Route("Product/Detail/{slug?}")]
        public async Task<IActionResult> Details(int? id, string slug)
        {
            ProductModel product = null;

            if (id.HasValue)
            {
                product = await _dataContext.Products
                    .Include(p => p.Brand)
                    .Include(p => p.Category)
                    .Include(p => p.Variants)
                    .FirstOrDefaultAsync(p => p.Id == id.Value);
            }
            else if (!string.IsNullOrEmpty(slug))
            {
                product = await _dataContext.Products
                    .Include(p => p.Brand)
                    .Include(p => p.Category)
                    .Include(p => p.Variants)
                    .FirstOrDefaultAsync(p => p.Slug == slug);
            }

            if (product == null)
                return RedirectToAction("Index", "Home");

            // ✅ Gợi ý sản phẩm liên quan
            var recommendedProducts = await _dataContext.Products
                .Where(p => p.Id != product.Id && p.CategoryId == product.CategoryId)
                .OrderByDescending(p => p.Id)
                .Take(6)
                .ToListAsync();

            if (recommendedProducts.Count < 6)
            {
                var remainingCount = 6 - recommendedProducts.Count;
                var additionalProducts = await _dataContext.Products
                    .Where(p => p.Id != product.Id
                                && p.CategoryId != product.CategoryId
                                && !recommendedProducts.Select(r => r.Id).Contains(p.Id))
                    .OrderByDescending(p => p.Id)
                    .Take(remainingCount)
                    .ToListAsync();

                recommendedProducts.AddRange(additionalProducts);
            }

            ViewBag.RecommendedProducts = recommendedProducts;
            return View(product);
        }

        // ✅ Hiển thị danh sách sản phẩm theo thương hiệu
        public async Task<IActionResult> ByBrand(string slug)
        {
            var brand = await _dataContext.Brands.FirstOrDefaultAsync(b => b.Slug == slug);
            if (brand == null)
                return RedirectToAction("Index", "Home");

            var products = await _dataContext.Products
                .Where(p => p.BrandId == brand.Id)
                .Include(p => p.Variants)
                .ToListAsync();

            ViewBag.Brand = brand;
            return View(products);
        }

        // ✅ Tìm kiếm sản phẩm
        [HttpGet]
        public async Task<IActionResult> Search(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return RedirectToAction("Index");

            var products = await _dataContext.Products
                .Where(p => p.Name.Contains(keyword))
                .ToListAsync();

            ViewBag.Keyword = keyword;
            return View(products);
        }
    }
}
