using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MYMVCAPP.Models
{
    [Table("Menu")]
    public class MenuModel
    {
        [Key]
        public int Id { get; set; }

        [StringLength(255)]
        public string Name { get; set; }

        public string? Slug { get; set; }

        public bool? Is_active { get; set; }

        public string? Description { get; set; }

        // ✅ Quan hệ 1-n với CategoryModel
        public ICollection<CategoryModel>? Categories { get; set; }
        
    }
}
