using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MYMVCAPP.Models; 

namespace MYMVCAPP.Models
{
    public class OrderItem
    {
        [Key]
        public int Id { get; set; }

        public int OrderId { get; set; }   // 🔹 chỉ có 1 foreign key
        public Order? Order { get; set; }  // 🔹 thuộc tính điều hướng

        public int ProductId { get; set; }

        [Required, MaxLength(300)]
        public string ProductName { get; set; } = null!;

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal LineTotal => Quantity * UnitPrice;


        // 🔗 Liên kết tới bảng Products
        [ForeignKey("ProductId")]
        public ProductModel Product { get; set; }
         [MaxLength(50)]
        public string? Color { get; set; }

        [MaxLength(50)]
        public string? Weight { get; set; }

        
    }
}
