using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MYMVCAPP.Repository;
using MYMVCAPP.Models;
using System.Threading.Tasks;
using System.Linq;

namespace MYMVCAPP.Controllers
{
    public class CategoryController : Controller
    {
        private readonly DataContext _dataContext;

        public CategoryController(DataContext context)
        {
            _dataContext = context;
        }

        // /Category/{slug}
        [Route("category/{slug}")]
        public async Task<IActionResult> Index(string slug = "")
        {
            if (string.IsNullOrEmpty(slug))
                return RedirectToAction("Index", "Home");

            var category = await _dataContext.Categories
                .FirstOrDefaultAsync(c => c.Slug == slug);

            if (category == null)
                return RedirectToAction("Index", "Home");

            var productsByCategory = await _dataContext.Products
                .Where(p => p.CategoryId == category.Id)
                .ToListAsync();

            ViewBag.CategoryName = category.Name;
            return View(productsByCategory);
        }
    }
}
