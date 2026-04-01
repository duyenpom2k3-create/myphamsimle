using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MYMVCAPP.Models;
using MYMVCAPP.Repository;
using System.Linq;

namespace MYMVCAPP.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataContext _dataContext;

        public HomeController(DataContext context)
        {
            _dataContext = context;
        }

        // GET: Home/Index
        public IActionResult Index()
        {
            // Lấy danh sách sản phẩm kèm Category và Brand
            var products = _dataContext.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .OrderByDescending(p => p.Id)
                .ToList();
                
            return View(products); // trả về IEnumerable<ProductModel>

        }

        // GET: Home/Details/5
        public IActionResult Details(int id)
        {
            var product = _dataContext.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Home/Privacy
        public IActionResult Privacy()
        {
            return View();
        }
        // 🟢 Action Tìm kiếm sản phẩm
        public async Task<IActionResult> Search(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                TempData["WarningMessage"] = "Vui lòng nhập từ khóa tìm kiếm.";
                return RedirectToAction("Index");
            }

            var results = await _dataContext.Products
                .Where(p => EF.Functions.Like(p.Name, $"%{keyword}%"))
                .Include(p => p.Category)
                .ToListAsync();

            ViewData["Keyword"] = keyword;
            return View(results);
        }
        
    }
}
