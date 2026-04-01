using Microsoft.AspNetCore.Mvc;
using MYMVCAPP.Repository;
using MYMVCAPP.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace MYMVCAPP.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HeaderController : Controller
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _env;

        public HeaderController(DataContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public IActionResult Index()
        {
            var headers = _context.Headers.ToList(); // DbSet<HeaderModel>
            return View(headers);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(HeaderModel model, IFormFile LogoFile)
        {
            if (ModelState.IsValid)
            {
                if (LogoFile != null)
                {
                    string uploadPath = Path.Combine(_env.WebRootPath, "images/home");
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(LogoFile.FileName);
                    string filePath = Path.Combine(uploadPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        LogoFile.CopyTo(stream);
                    }
                    model.Logo = fileName;
                }

                _context.Headers.Add(model);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public IActionResult Edit(int id)
        {
            var header = _context.Headers.Find(id);
            if (header == null) return NotFound();
            return View(header);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, HeaderModel model, IFormFile LogoFile)
        {
            var header = _context.Headers.Find(id);
            if (header == null) return NotFound();

            header.Name = model.Name;
            header.Link = model.Link;
            header.Note = model.Note;

            if (LogoFile != null)
            {
                string uploadPath = Path.Combine(_env.WebRootPath, "images/home");
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(LogoFile.FileName);
                string filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    LogoFile.CopyTo(stream);
                }
                header.Logo = fileName;
            }

            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var header = _context.Headers.Find(id);
            if (header == null) return NotFound();

            _context.Headers.Remove(header);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
