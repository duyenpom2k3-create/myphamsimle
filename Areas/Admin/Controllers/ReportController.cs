using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MYMVCAPP.Models.ViewModels;
using MYMVCAPP.Repository;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.IO;                 // thêm
using iTextSharp.text;          // thêm
using iTextSharp.text.pdf;      // thêm

namespace MYMVCAPP.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ReportController : Controller
    {
        private readonly DataContext _context;

        public ReportController(DataContext context)
        {
            _context = context;
        }

        // GET: Admin/Report/Compare
// GET: Admin/Report/Compare
public async Task<IActionResult> Index(int? yearNow, int? yearPrev)
{
    int thisYear = yearNow ?? DateTime.Now.Year;
    int prevYear = yearPrev ?? DateTime.Now.Year - 1;

    // ✅ Lấy thông tin công ty từ bảng FooterItems
    var companyInfo = await _context.FooterItems
        .Where(f => f.ItemType == "COMPANY")
        .ToDictionaryAsync(f => f.ItemKey, f => f.ItemTitle);

    // ✅ Gửi sang View qua ViewBag
    ViewBag.Company = companyInfo;

    // Lấy doanh thu theo danh mục
    var categoryReports = await _context.Categories
        .Select(cat => new CategoryReport
        {
            CategoryName = cat.Name,
            RevenueThisYear = _context.OrderItems
                .Where(i => i.Product.CategoryId == cat.Id &&
                            i.Order.CreatedAt.Year == thisYear)
                .Sum(i => (decimal?)i.Quantity * i.UnitPrice) ?? 0,

            RevenueLastYear = _context.OrderItems
                .Where(i => i.Product.CategoryId == cat.Id &&
                            i.Order.CreatedAt.Year == prevYear)
                .Sum(i => (decimal?)i.Quantity * i.UnitPrice) ?? 0
        })
        .ToListAsync();

    var model = new ReportCompareModel
    {
        YearNow = thisYear,
        YearPrev = prevYear,
        CategoryReports = categoryReports
    };

    return View(model);
}


        public async Task<IActionResult> ExportRevenuePdf(int? year, int? month)
        {
            year ??= DateTime.Now.Year;
            month ??= DateTime.Now.Month;

            var orders = await _context.Orders   // đổi _dataContext -> _context
                .Where(o => o.CreatedAt.Year == year && o.CreatedAt.Month == month)
                .ToListAsync();

            var totalRevenue = orders.Sum(o => o.TotalAmount);

            using (var ms = new MemoryStream())
            {
                var doc = new Document(PageSize.A4, 40, 40, 40, 40);
                PdfWriter.GetInstance(doc, ms);
                doc.Open();

                doc.Add(new Paragraph($"BÁO CÁO DOANH THU THÁNG {month}/{year}") { Alignment = Element.ALIGN_CENTER });
                doc.Add(new Paragraph($"Tổng doanh thu: {totalRevenue:N0} ₫"));
                doc.Add(new Paragraph($"Tổng đơn hàng: {orders.Count}"));
                doc.Add(new Paragraph(" "));

                var table = new PdfPTable(3);
                table.AddCell("Tên khách hàng");
                table.AddCell("Ngày tạo");
                table.AddCell("Tổng tiền");

                foreach (var o in orders)
                {
                    table.AddCell(o.FullName ?? "");
                    table.AddCell(o.CreatedAt.ToString("dd/MM/yyyy"));
                    table.AddCell(o.TotalAmount.ToString("N0"));
                }

                doc.Add(table);
                doc.Close();

                return File(ms.ToArray(), "application/pdf", $"BaoCao_DoanhThu_{month}_{year}.pdf");
            }
            
        }
    }
}
