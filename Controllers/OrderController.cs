using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MYMVCAPP.Models;
using MYMVCAPP.Repository;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace MyMvcApp.Controllers
{
    public class OrderController : Controller
    {
        private readonly DataContext _context;

        public OrderController(DataContext context)
        {
            _context = context;
        }

        // GET: /Order/MyOrders?email=user@gmail.com
        public async Task<IActionResult> MyOrders(string email)
        {
            var orders = await _context.Orders
                .Where(o => o.Email == email)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return View(orders);
        }

        // GET: /Order/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound();

            return View(order);
        }
[HttpPost]
        public IActionResult CancelOrder(int id, string email)
        {
            var order = _context.Orders.FirstOrDefault(o => o.Id == id);
            if (order == null)
                return NotFound();

            // Kiểm tra trạng thái
            if (order.Status == "Đã giao" || order.Status == "Đã hủy")
            {
                TempData["ErrorMessage"] = "Đơn hàng này không thể hủy.";
                return RedirectToAction("Details", new { id });
            }

            order.Status = "Đã hủy";
            order.UpdatedAt = DateTime.Now;

            _context.Update(order);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Đơn hàng đã được hủy thành công.";

            // Lấy lại danh sách đơn hàng của người dùng
            var orders = _context.Orders
                .Where(o => o.Email == email)
                .OrderByDescending(o => o.CreatedAt)
                .ToList();

            return View("~/Views/User/Orders.cshtml", orders);
        }
    }
}
