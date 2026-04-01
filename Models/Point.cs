using System;

namespace MYMVCAPP.Models
{
    public class Point
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int OrderId { get; set; }
        public int Points { get; set; } // số điểm (+ hoặc -)
        public string Description { get; set; } // ví dụ "Đơn hàng #123"
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public UserModel User { get; set; } // navigation property
        public Order Order { get; set; } // navigation property
    }
}
