using Microsoft.AspNetCore.Mvc;
using MYMVCAPP.Models;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using MYMVCAPP.Repository;

namespace MYMVCAPP.ViewComponents
{
    public class UserSidebarViewComponent : ViewComponent
    {
        private readonly DataContext _context;

        public UserSidebarViewComponent(DataContext context)
        {
            _context = context;
        }

        // Đây là phương thức bạn vừa có
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var menuItems = await _context.MenuUsers
                                          .Where(m => m.Is_active == true)
                                          .OrderBy(m => m.Id)
                                          .ToListAsync();
            return View(menuItems); // Trả về View của ViewComponent
        }
    }
}
