using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MYMVCAPP.Models;
using MYMVCAPP.Repository;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace MYMVCAPP.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly DataContext _dataContext;

        public UserController(DataContext context)
        {
            _dataContext = context;
        }

        // ================= Index =================
        public async Task<IActionResult> Index()
        {
            var users = await _dataContext.Users
                .OrderByDescending(u => u.Id)
                .ToListAsync();
            return View(users);
        }

        // ================= Create =================
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Roles = new List<SelectListItem>
            {
                new SelectListItem { Text = "User", Value = "0" },
                new SelectListItem { Text = "Admin", Value = "1" }
            };
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserModel user, IFormFile AvatarFile)
        {
            ViewBag.Roles = new List<SelectListItem>
            {
                new SelectListItem { Text = "User", Value = "0" },
                new SelectListItem { Text = "Admin", Value = "1" }
            };

            if (!ModelState.IsValid)
                return View(user);

            if (await _dataContext.Users.AnyAsync(u => u.Email == user.Email))
            {
                ModelState.AddModelError("", "Người dùng đã tồn tại!");
                return View(user);
            }

            // Tạo folder nếu chưa tồn tại
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/avatars");
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            // Xử lý avatar
            if (AvatarFile != null && AvatarFile.Length > 0)
            {
                var fileName = Path.GetFileName(AvatarFile.FileName);
                var filePath = Path.Combine(uploadPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await AvatarFile.CopyToAsync(stream);
                }
                user.Avatar = fileName; // chỉ lưu tên file
            }

            _dataContext.Users.Add(user);
            await _dataContext.SaveChangesAsync();
            TempData["success"] = "Thêm người dùng thành công!";
            return RedirectToAction(nameof(Index));
        }

        // ================= Edit =================
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _dataContext.Users.FindAsync(id);
            if (user == null) return NotFound();

            ViewBag.Roles = new List<SelectListItem>
            {
                new SelectListItem { Text = "User", Value = "0" },
                new SelectListItem { Text = "Admin", Value = "1" }
            };

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UserModel model, IFormFile AvatarFile)
        {
            if (id != model.Id)
                return BadRequest();

            var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return NotFound();

            if (!ModelState.IsValid)
                return View(model);

            // Cập nhật các field
            user.Username = model.Username;
            user.Email = model.Email;
            user.Role = model.Role;

            // Giữ password cũ
            user.Password = user.Password;

            // Xử lý avatar
            if (AvatarFile != null && AvatarFile.Length > 0)
            {
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/avatars");
                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                var fileName = Path.GetFileName(AvatarFile.FileName);
                var filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await AvatarFile.CopyToAsync(stream);
                }

                user.Avatar = fileName; // chỉ lưu tên file
            }

            try
            {
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Cập nhật người dùng thành công!";
            }
            catch (DbUpdateException)
            {
                TempData["error"] = "Có lỗi khi cập nhật dữ liệu!";
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        // ================= Delete =================
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _dataContext.Users.FindAsync(id);
            if (user == null) return NotFound();

            _dataContext.Users.Remove(user);
            await _dataContext.SaveChangesAsync();
            TempData["success"] = "Xóa người dùng thành công!";
            return RedirectToAction(nameof(Index));
        }

        // ================= GetUserDisplayAsync =================
        public async Task<(string Username, string Avatar)> GetUserDisplayAsync(int? userId)
        {
            string userAvatar = "/images/avatars/avatar_3.jpg"; // avatar mặc định
            string username = "Khách"; // tên mặc định

            if (userId.HasValue)
            {
                var user = await _dataContext.Users.FindAsync(userId.Value);
                if (user != null)
                {
                    username = user.Username;
                    if (!string.IsNullOrEmpty(user.Avatar))
                    {
                        if (!user.Avatar.StartsWith("/images/avatars/"))
                            userAvatar = "/images/avatars/" + user.Avatar;
                        else
                            userAvatar = user.Avatar;
                    }
                }
            }

            return (username, userAvatar);
        }
    }
}
