using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MYMVCAPP.Repository;
using MYMVCAPP.Models;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;

namespace MYMVCAPP.Controllers
{
    public class UserController : Controller
    {
        private readonly DataContext _context;

        public UserController(DataContext context)
        {
            _context = context;
        }

        // Trang hồ sơ người dùng
        public async Task<IActionResult> Index()
        {
            // Kiểm tra session
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Lấy thông tin người dùng
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return NotFound();
            }

            // Lấy danh sách đơn hàng của người dùng (nếu bạn có Email trong đơn hàng)
            var orders = await _context.Orders
                .Where(o => o.Email == user.Email)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            // Gửi dữ liệu sang View
            ViewBag.Orders = orders;
            return View(user);
        }

        // Cập nhật thông tin người dùng
        [HttpPost]
        public async Task<IActionResult> Update(UserModel model)
        {
            if (!ModelState.IsValid)
                return View("Index", model);

            var user = await _context.Users.FindAsync(model.Id);
            if (user == null)
                return NotFound();

            user.Username = model.Username;
            user.Email = model.Email;
            user.Password = model.Password;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            ViewBag.Message = "Cập nhật thông tin thành công!";
            return RedirectToAction("Index");
        }
        // GET: /User/Details
        public IActionResult Details()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                // Nếu chưa đăng nhập, redirect sang trang Login
                return RedirectToAction("Login", "Account");
            }

            var user = _context.Users
                .FirstOrDefault(u => u.Id == userId.Value);

            if (user == null)
            {
                return NotFound();
            }

            var model = new UserModel
            {
                Id = user.Id,
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                Address = user.Address,
                Province = user.Province,
                Role = user.Role
            };

            return View(model);
        }
        // Trang "Đơn hàng của tôi"
        public async Task<IActionResult> Orders()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Lấy email hoặc userId để lọc đơn hàng
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return NotFound();

            // Lấy danh sách đơn hàng của user
            var orders = await _context.Orders
                .Where(o => o.Email == user.Email) // hoặc dùng UserId nếu bạn thêm cột FK UserId
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return View(orders); // View: /Views/User/Orders.cshtml
        }
        public async Task<IActionResult> OrderDetails(int id)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            // Lấy đơn hàng kèm chi tiết sản phẩm
            var order = await _context.Orders
                .Include(o => o.Items) // OrderItems
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound();

            return View(order); // /Views/User/OrderDetails.cshtml
        }
        [HttpPost]
        public async Task<IActionResult> UploadAvatar(IFormFile AvatarFile)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            if (AvatarFile != null && AvatarFile.Length > 0)
            {
                // Lưu file vào wwwroot/images/avatars
                var fileName = $"avatar_{userId}{Path.GetExtension(AvatarFile.FileName)}";
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/avatars", fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await AvatarFile.CopyToAsync(stream);
                }

                // Cập nhật session + DB nếu cần
                HttpContext.Session.SetString("UserAvatar", "/images/avatars/" + fileName);

                var user = await _context.Users.FindAsync(userId.Value);
                if (user != null)
                {
                    user.Avatar = fileName;
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }

                TempData["Success"] = "Cập nhật ảnh đại diện thành công!";
            }
            else
            {
                TempData["Error"] = "Vui lòng chọn ảnh trước khi lưu.";
            }

            return RedirectToAction("Details", "User");
        }
        public async Task<IActionResult> Wishlist()
    {
        int? userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
            return RedirectToAction("Login", "Account");

        var wishlist = _context.Wishlist
            .Include(w => w.Product)
            .Where(w => w.UserId == userId)
            .ToList();

        return View("~/Views/User/Wishlist.cshtml", wishlist);
    }
public async Task<IActionResult> FAQ()
        {
            // Kiểm tra session
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            // Lấy danh sách FAQ từ database (nếu bạn có bảng FAQ)
            var faqs = await _context.FAQs
                .OrderBy(f => f.Order)
                .ToListAsync();

            return View(faqs); // View: /Views/User/FAQ.cshtml
        }


    }
}
