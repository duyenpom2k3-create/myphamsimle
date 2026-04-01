using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MYMVCAPP.Repository;
using System.Linq;
using System.Threading.Tasks;

namespace MYMVCAPP.Repository.Components
{
    public class MenusViewComponent : ViewComponent
    {
        private readonly DataContext _context;

        public MenusViewComponent(DataContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // ✅ EF Core không hỗ trợ Include + Where trực tiếp trong Include
            // Cách đúng là Include toàn bộ, sau đó lọc trong bộ nhớ (LINQ to Objects)
            var menus = await _context.Menus
                .Include(m => m.Categories)
                .OrderBy(m => m.Id)
                .ToListAsync();

            // Lọc chỉ lấy Categories có Status = 1
            foreach (var menu in menus)
            {
                menu.Categories = menu.Categories
                    .Where(c => c.Status == 1)
                    .OrderBy(c => c.Id)
                    .ToList();
            }

            return View(menus);
        }
    }
}
