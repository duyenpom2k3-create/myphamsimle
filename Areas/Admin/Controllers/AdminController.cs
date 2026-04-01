using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MYMVCAPP.Models;
using MYMVCAPP.Repository;
using System.Linq;
using System.Threading.Tasks;

namespace MYMVCAPP.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminController : Controller
    {
        private readonly DataContext _dataContext;

        public AdminController(DataContext context)
        {
            _dataContext = context;
        }

        // GET: Admin/Admin/Index
        public IActionResult Index()
        {
            // Tổng số sản phẩm
            var totalProducts = _dataContext.Products.Count();

            // Tổng số danh mục
            var totalCategories = _dataContext.Categories.Count();

            // Tổng số thương hiệu
            var totalBrands = _dataContext.Brands.Count();

            // Lấy danh sách sản phẩm gần đây nhất
            var recentProducts = _dataContext.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .OrderByDescending(p => p.Id)
                .Take(5)
                .ToList();

            // Truyền dữ liệu sang view bằng ViewBag
            ViewBag.TotalProducts = totalProducts;
            ViewBag.TotalCategories = totalCategories;
            ViewBag.TotalBrands = totalBrands;
            ViewBag.RecentProducts = recentProducts;

            return View();
        }

        // ================= Logout =================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // Đăng xuất khỏi cookie authentication
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home", new { area = "" });
        }
    }
}
