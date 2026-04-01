// Trong: /Models/Province.cs
using System.ComponentModel.DataAnnotations;

namespace MYMVCAPP.Models
{
    public class Province
    {
        [Key]
        public int Id { get; set; } // Khóa chính (ví dụ: 1, 2, 3...)

        [Required]
        [StringLength(100)]
        public string Name { get; set; } // Tên tỉnh, ví dụ: "Hà Nội"

        [Required]
        public decimal ShippingFee { get; set; } // Phí ship, ví dụ: 15000

    }
}