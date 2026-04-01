using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MYMVCAPP.Models
{
    public class UserAddress
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string FullName { get; set; } = null!;

        [MaxLength(200)]
        public string Email { get; set; } = null!;

        [Required, MaxLength(50)]
        public string Phone { get; set; } = null!;

        [Required, MaxLength(500)]
        public string Address { get; set; } = null!;

        [Required, MaxLength(100)]
        public string City { get; set; } = null!; // Hà Nội / Tỉnh khác

        public int? UserId { get; set; } // optional nếu guest
        [ForeignKey("UserId")]
        public UserModel? User { get; set; }

        public bool IsDefault { get; set; } = false;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        

    }
}
