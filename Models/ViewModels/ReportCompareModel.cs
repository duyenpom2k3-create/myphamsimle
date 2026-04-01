using System.Collections.Generic;

namespace MYMVCAPP.Models.ViewModels
{
    public class ReportCompareModel
    {
        public int YearNow { get; set; }
        public int YearPrev { get; set; }

        // Danh sách doanh thu theo danh mục
        public List<CategoryReport> CategoryReports { get; set; } = new();
        
    }

    public class CategoryReport
    {
        public string CategoryName { get; set; }
        public decimal RevenueThisYear { get; set; }
        public decimal RevenueLastYear { get; set; }
        public decimal Difference => RevenueThisYear - RevenueLastYear;
        public decimal Total => RevenueThisYear + RevenueLastYear;
          public List<CategoryReport> CategoryReports { get; set; } = new List<CategoryReport>();
    }
}
