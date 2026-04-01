using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MYMVCAPP.Models;
using MYMVCAPP.Models.ViewModels;
using MYMVCAPP.Repository;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MYMVCAPP.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class WishlistController : Controller
    {
        private readonly DataContext _dataContext;

        public WishlistController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        // ================= Index: wishlist của user =================
        // Hiển thị wishlist của 1 user (mặc định userId = 1)
        public async Task<IActionResult> Index(int? userId)
{
    // Danh sách user để dropdown
    var users = await _dataContext.Users.OrderBy(u => u.Id).ToListAsync();
    ViewBag.Users = new SelectList(users, "Id", "Username", userId);

    if (userId == null)
    {
        userId = users.FirstOrDefault()?.Id ?? 0;
    }

    // Lấy wishlist user
    var wishlist = await _dataContext.Wishlist
        .Include(w => w.Product)
        .Where(w => w.UserId == userId)
        .OrderByDescending(w => w.CreatedAt)
        .ToListAsync();

    ViewBag.UserId = userId;

    // Lấy top sản phẩm được yêu thích
    var topProducts = await _dataContext.Wishlist
        .Include(w => w.Product)
        .GroupBy(w => w.ProductId)
        .Select(g => new TopWishlistViewModel
        {
            Product = g.First().Product,
            Count = g.Count()
        })
        .OrderByDescending(x => x.Count)
        .Take(10)
        .ToListAsync();

    ViewBag.TopWishlist = topProducts;

    return View(wishlist);
}


        // ================= Top sản phẩm được yêu thích =================
        public async Task<IActionResult> TopWishlist(int top = 10)
        {
            var topProducts = await _dataContext.Wishlist
                .Include(w => w.Product)
                .GroupBy(w => w.ProductId)
                .Select(g => new TopWishlistViewModel
                {
                    Product = g.First().Product,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .Take(top)
                .ToListAsync();

            return View(topProducts); // -> Views/Areas/Admin/Wishlist/TopWishlist.cshtml
        }

        // ================= Thêm sản phẩm vào wishlist =================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToWishlist(int userId, int productId)
        {
            if (!await _dataContext.Wishlist.AnyAsync(w => w.UserId == userId && w.ProductId == productId))
            {
                _dataContext.Wishlist.Add(new WishlistModel
                {
                    UserId = userId,
                    ProductId = productId
                });
                await _dataContext.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index), new { userId });
        }

        // ================= Xóa sản phẩm khỏi wishlist =================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromWishlist(int id, int userId)
        {
            var item = await _dataContext.Wishlist.FindAsync(id);
            if (item != null)
            {
                _dataContext.Wishlist.Remove(item);
                await _dataContext.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index), new { userId });
        }
    }
}
