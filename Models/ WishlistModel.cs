using System.ComponentModel.DataAnnotations;
using MYMVCAPP.Models; 

public class WishlistModel
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ProductId { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation property
    public UserModel User { get; set; }
    public ProductModel Product { get; set; }
}
