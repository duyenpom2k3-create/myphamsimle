using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MYMVCAPP.Models
{
    [Table("Categories")]
    public class CategoryModel
    {
        [Key]
        public int Id { get; set; }

        [Required, MinLength(4, ErrorMessage = "Yêu cầu nhập tên danh mục")]
        [StringLength(255)]
        public string Name { get; set; }

        [Required, MinLength(4, ErrorMessage = "Yêu cầu nhập mô tả danh mục")]
        public string Description { get; set; }

        [StringLength(255)]
        public string? Slug { get; set; }

        public int Status { get; set; }

        // ✅ Khóa ngoại đến MenuModel
        public int? MenuId { get; set; }

        [ForeignKey("MenuId")]
        public MenuModel? Menu { get; set; }
    }
}
