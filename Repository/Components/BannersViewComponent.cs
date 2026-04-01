using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MYMVCAPP.Repository;

namespace MYMVCAPP.Repository.Components
{
    public class BannersViewComponent : ViewComponent
    {
        private readonly DataContext _context;

        public BannersViewComponent(DataContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Lấy banner đang kích hoạt, sắp xếp theo thứ tự
            var banners = await _context.Banners
                .Where(b => b.Is_Active == true)
                .OrderBy(b => b.Sort_Order)
                .ToListAsync();

            return View(banners);
        }
    }
}
