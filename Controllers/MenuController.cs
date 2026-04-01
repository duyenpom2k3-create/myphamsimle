using Microsoft.AspNetCore.Mvc;
using MYMVCAPP.Repository;
using System.Linq;

namespace MYMVCAPP.Controllers
{
    public class MenuController : Controller
    {
        private readonly DataContext _context;

        public MenuController(DataContext context)
        {
            _context = context;
        }

        public IActionResult Details(string slug)
        {
            var menu = _context.Menus
                .Where(m => m.Slug == slug)
                .FirstOrDefault();

            if (menu == null)
                return NotFound();

            return View(menu);
        }
        
    }
}
