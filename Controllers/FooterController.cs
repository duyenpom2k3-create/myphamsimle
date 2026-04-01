using Microsoft.AspNetCore.Mvc;
using MYMVCAPP.Repository;
using MYMVCAPP.Models;
using System.Linq;

namespace MYMVCAPP.Controllers
{
    public class FooterController : Controller
    {
        private readonly DataContext _context;

        public FooterController(DataContext context)
        {
            _context = context;
        }

        // 🔹 Hiển thị chi tiết theo slug (VD: /gioi-thieu)
        [Route("{slug}")]
        public IActionResult Detail(string slug)
        {
            if (string.IsNullOrEmpty(slug))
                return NotFound();

            var item = _context.FooterItems.FirstOrDefault(x => x.Slug == slug);
            if (item == null)
                return NotFound();

            // ✅ Nếu muốn kiểm tra nhanh:
            // return Content($"Đã tìm thấy item: {item.ItemTitle}");

            // ✅ Khi chạy thật, trả về view /Views/Footer/Detail.cshtml
            return View(item);
        }
    }
}
