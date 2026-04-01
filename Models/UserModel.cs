using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MYMVCAPP.Models
{
    public class UserModel
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Username { get; set; } = null!;

        [Required, MaxLength(200)]
        public string Email { get; set; } = null!;

        [Required, MaxLength(200)]
        public string Password { get; set; } = null!;

        // 1 = Admin, 0 = User
        public int Role { get; set; }

        // ===== Thông tin người nhận (tùy chọn) =====
        [MaxLength(200)]
        public string? FullName { get; set; }

        [MaxLength(50)]
        public string? Phone { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }

        [MaxLength(100)]
        public string? Province { get; set; }
        public string Avatar { get; set; }

        // ✅ Quan hệ 1-nhiều: Một user có nhiều địa chỉ
        public ICollection<UserAddress>? Addresses { get; set; } = new List<UserAddress>();
        public ICollection<WishlistModel> Wishlists { get; set; } = new List<WishlistModel>();
       
    }
}
