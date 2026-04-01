using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MYMVCAPP.Models;
using MYMVCAPP.Repository;
using System.Threading.Tasks;
using System.Linq;

namespace MYMVCAPP.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MenuUserController : Controller
    {
             private readonly DataContext _dataContext;

        public MenuUserController(DataContext context)
        {
            _dataContext = context;
        }

        public IActionResult Index()
        {
            var data = _dataContext.MenuUsers.ToList();
            return View(data);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(MenuUserModel model)
        {
            if (ModelState.IsValid)
            {
                _dataContext.MenuUsers.Add(model);
                _dataContext.SaveChanges();
                TempData["success"] = "Thêm menu thành công!";
                return RedirectToAction("Index");
            }

            TempData["error"] = "Lỗi! Vui lòng thử lại.";
            return View(model);
        }

        public IActionResult Edit(int id)
        {
            var menu = _dataContext.MenuUsers.Find(id);
            return View(menu);
        }

        [HttpPost]
        public IActionResult Edit(MenuUserModel model)
        {
            _dataContext.MenuUsers.Update(model);
            _dataContext.SaveChanges();
            TempData["success"] = "Cập nhật menu thành công!";
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var menu = _dataContext.MenuUsers.Find(id);
            _dataContext.MenuUsers.Remove(menu);
            _dataContext.SaveChanges();
            TempData["success"] = "Xóa thành công!";
            return RedirectToAction("Index");
        }
    }
}
