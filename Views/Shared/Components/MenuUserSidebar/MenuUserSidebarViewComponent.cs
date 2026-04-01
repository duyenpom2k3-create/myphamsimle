using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MYMVCAPP.Repository;

namespace MYMVCAPP.ViewComponents
{
    public class MenuUserSidebarViewComponent : ViewComponent
    {
        private readonly DataContext _context;

        public MenuUserSidebarViewComponent(DataContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var menus = await _context.MenuUsers
                .Where(m => m.Is_active == true)
                .OrderBy(m => m.Id)
                .ToListAsync();

            return View(menus);
        }
    }
}
