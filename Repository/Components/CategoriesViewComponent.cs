using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MYMVCAPP.Repository;

namespace MYMVCAPP.Repository.Components
{
    public class CategoriesViewComponent : ViewComponent
    {
        private readonly DataContext _dataContext;
        public CategoriesViewComponent(DataContext context)
        {
            _dataContext = context;
        }
       public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = await _dataContext.Categories.ToListAsync(); // ✅ có await
            return View(categories);
        }

    }
}
