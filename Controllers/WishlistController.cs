using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MYMVCAPP.Repository;
using MYMVCAPP.Models;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace MYMVCAPP.Controllers
{
    public class WishlistController : Controller
    {
        private readonly DataContext _context;

        public WishlistController(DataContext context)
        {
            _context = context;
        }

        // Thêm sản phẩm vào yêu thích
        [HttpPost]
        public async Task<IActionResult> AddToWishlist(int productId)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["Error"] = "Vui lòng đăng nhập để thêm sản phẩm yêu thích.";
                return RedirectToAction("Login", "Account");
            }

            var existing = await _context.Wishlist
                .FirstOrDefaultAsync(w => w.ProductId == productId && w.UserId == userId);

            if (existing != null)
            {
                TempData["Info"] = "Sản phẩm đã có trong danh sách yêu thích.";
                return RedirectToAction("Wishlist");
            }

            var wishlistItem = new WishlistModel
            {
                UserId = userId.Value,
                ProductId = productId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Wishlist.Add(wishlistItem);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Đã thêm sản phẩm vào danh sách yêu thích!";
            return RedirectToAction("Wishlist");
        }

        // Hiển thị danh sách yêu thích
        [HttpGet]
        public IActionResult Wishlist()
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

        // Xoá sản phẩm khỏi wishlist
        [HttpGet]
        public async Task<IActionResult> Remove(int id)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var item = await _context.Wishlist
                .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);

            if (item != null)
            {
                _context.Wishlist.Remove(item);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Đã xoá sản phẩm khỏi danh sách yêu thích!";
            }
            else
            {
                TempData["Error"] = "Không tìm thấy sản phẩm để xoá.";
            }

            return RedirectToAction("Wishlist");
        }
    }
}
