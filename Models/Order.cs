using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MYMVCAPP.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string FullName { get; set; } = null!;

        [Required, MaxLength(200)]
        public string Email { get; set; } = null!;

        [Required, MaxLength(50)]
        public string Phone { get; set; } = null!;

        [Required, MaxLength(500)]
        public string Address { get; set; } = null!;

        [Required, MaxLength(50)]
        public string PaymentMethod { get; set; } = "COD";

        [Column(TypeName = "decimal(18,2)")]
        public decimal SubTotal { get; set; } // Tổng tiền hàng

        [Column(TypeName = "decimal(18,2)")]
        public decimal ShippingFee { get; set; } = 0m; // ✅ Giữ lại bản này thôi

        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountAmount { get; set; } = 0m; // Giảm giá

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; } // Tổng thanh toán

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(50)]
        public string Status { get; set; } = "Pending";
        public DateTime? UpdatedAt { get; set; }


        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}
