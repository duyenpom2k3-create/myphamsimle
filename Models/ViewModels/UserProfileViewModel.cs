using System.Collections.Generic;

namespace MYMVCAPP.Models
{
    public class UserProfileViewModel
    {
        public UserModel User { get; set; }
        public List<Order> Orders { get; set; } = new();
    }
}
