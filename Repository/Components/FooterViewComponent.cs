using Microsoft.AspNetCore.Mvc;
using MYMVCAPP.Models;
using MYMVCAPP.Repository;
using System.Linq;

namespace MYMVCAPP.ViewComponents
{
    public class FooterViewComponent : ViewComponent
    {
        private readonly DataContext _context;

        public FooterViewComponent(DataContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            var model = new FooterViewModel
            {
                CompanyItems = _context.FooterItems
                    .Where(f => f.ItemType == "COMPANY" )
                    .OrderBy(f => f.DisplayOrder)
                    .ToList(),

                PolicyItems = _context.FooterItems
                    .Where(f => f.ItemType == "POLICY" )
                    .OrderBy(f => f.DisplayOrder)
                    .ToList(),

                FollowItems = _context.FooterItems
                    .Where(f => f.ItemType == "FOLLOW" )
                    .OrderBy(f => f.DisplayOrder)
                    .ToList(),

                Categories = _context.Categories
                    .OrderBy(c => c.Name)
                    .ToList()
            };

            return View(model);
        }
    }
}
