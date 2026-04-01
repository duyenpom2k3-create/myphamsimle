using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MYMVCAPP.Repository;

namespace MYMVCAPP.Repository.Components
{
    public class BrandsViewComponent : ViewComponent
    {
        private readonly DataContext _dataContext;
        public BrandsViewComponent(DataContext context)
        {
            _dataContext = context;
        }
       public async Task<IViewComponentResult> InvokeAsync()
        {
            var brands = await _dataContext.Brands.ToListAsync(); // ✅ có await
            return View(brands);
        }

    }
}
