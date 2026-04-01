using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MYMVCAPP.Models;
using MYMVCAPP.Models.ViewModels;
using MYMVCAPP.Repository;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace MYMVCAPP.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController : Controller
    {
        private readonly DataContext _dataContext;

        public OrderController(DataContext context)
        {
            _dataContext = context;
        }

        // ✅ GET: Admin/Order
        public async Task<IActionResult> Index()
        {
            // Lấy toàn bộ danh sách đơn hàng (mới nhất trước)
            var orders = await _dataContext.Orders
                .Include(o => o.Items)
                .OrderByDescending(o => o.Id)
                .ToListAsync();

            return View(orders);
        }

        // ✅ GET: Admin/Order/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var order = await _dataContext.Orders
                .Include(o => o.Items) // load chi tiết đơn hàng
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();

            // ✅ Lấy thông tin công ty từ FooterItems
            var companyInfo = await _dataContext.FooterItems
                .Where(f => f.ItemType == "COMPANY")
                .ToDictionaryAsync(f => f.ItemKey, f => f.ItemTitle);

            // Gửi sang View qua ViewBag
            ViewBag.Company = companyInfo;

            return View(order);
        }

        // ✅ GET: Admin/Order/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var order = await _dataContext.Orders.FindAsync(id);
            if (order == null) return NotFound();

            return View(order);
        }

        // ✅ POST: Admin/Order/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Order order)
        {
            if (id != order.Id) return BadRequest();

            if (!ModelState.IsValid) return View(order);

            try
            {
                _dataContext.Orders.Update(order);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Cập nhật đơn hàng thành công!";
            }
            catch (DbUpdateException)
            {
                TempData["error"] = "Lỗi khi cập nhật đơn hàng!";
            }

            return RedirectToAction(nameof(Index));
        }

        // ✅ GET: Admin/Order/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _dataContext.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();

            _dataContext.OrderItems.RemoveRange(order.Items); // xóa chi tiết đơn
            _dataContext.Orders.Remove(order);
            await _dataContext.SaveChangesAsync();

            TempData["success"] = "Xóa đơn hàng thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}
