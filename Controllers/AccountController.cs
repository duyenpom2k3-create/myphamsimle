using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MYMVCAPP.Repository;
using MYMVCAPP.Models;
using MYMVCAPP.Models.ViewModels;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace MYMVCAPP.Controllers
{
    public class AccountController : Controller
    {
        private readonly DataContext _context;

        public AccountController(DataContext context)
        {
            _context = context;
        }

        // GET: /Account/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserModel user, string passwordConfirm)
        {
            if (!ModelState.IsValid)
                return View(user);

            if (user.Password != passwordConfirm)
            {
                ModelState.AddModelError("", "Mật khẩu xác nhận không khớp");
                return View(user);
            }

            var exists = await _context.Users.AnyAsync(u => u.Username == user.Username || u.Email == user.Email);
            if (exists)
            {
                ModelState.AddModelError("", "Tên đăng nhập hoặc email đã tồn tại");
                return View(user);
            }

            // Mặc định role = 0 (User)
            user.Role = 0;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            TempData["success"] = "Đăng ký thành công. Vui lòng đăng nhập!";
            return RedirectToAction("Login");
        }

        // GET: /Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username && u.Password == password);
            if (user == null)
            {
                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng");
                return View();
            }

            // Lưu session
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetInt32("Role", user.Role);

            if (user.Role == 1)
                return RedirectToAction("Index", "Admin", new { area = "Admin" });
            else
                return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
                // Trang Mua Lại
        public async Task<IActionResult> Reorder()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Lấy danh sách đơn hàng của người dùng
            var orders = await _context.Orders
                .Where(o => o.Id == userId)
                .Include(o => o.Items) // nếu bạn có bảng OrderItems
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return View(orders);
        }

        // Action thêm lại sản phẩm từ đơn hàng vào giỏ
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ReorderItems(int orderId)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var order = _context.Orders
                .Include(o => o.Items)
                .FirstOrDefault(o => o.Id == orderId && o.Id == userId);

            if (order == null || order.Items == null)
            {
                TempData["Error"] = "Đơn hàng không tồn tại hoặc không có sản phẩm để đặt lại.";
                return RedirectToAction("Reorder");
            }

            // Lấy giỏ hàng từ session
            // Session giỏ hàng
            var cart = HttpContext.Session.GetJson<CartItemViewModel>("Cart");

            if (cart == null)
            {
                cart = new CartItemViewModel
                {
                    CartItems = new List<CartItemModel>(),
                    GrandTotal = 0
                };
            }


            // Thêm sản phẩm từ đơn cũ vào giỏ
            foreach (var item in order.Items)
            {
                cart.CartItems.Add(new CartItemModel
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    Quantity = item.Quantity,
                    Price = item.UnitPrice
                });
            }

            // Cập nhật session
            HttpContext.Session.SetJson("Cart", cart);

            TempData["Success"] = "Các sản phẩm từ đơn hàng đã được thêm vào giỏ hàng.";
            return RedirectToAction("Index", "Cart");
        }
    }
        
    }
