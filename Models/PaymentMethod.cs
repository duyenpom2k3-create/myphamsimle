using System.ComponentModel.DataAnnotations;

namespace MYMVCAPP.Models
{
    public class PaymentMethod
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } // Tên hình thức thanh toán (VD: Thanh toán khi nhận hàng, Chuyển khoản, VNPay...)

        [StringLength(255)]
        public string Description { get; set; }

        public ICollection<Order> Orders { get; set; }
    }
}
