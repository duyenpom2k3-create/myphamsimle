namespace MYMVCAPP.Models.ViewModels
{
    public class CartItemViewModel
    {
       public List<CartItemModel> CartItems { get; set; } = new List<CartItemModel>();
        public decimal GrandTotal { get; set; } = 0;        
      // CÁC THUỘC TÍNH MỚI DÙNG ĐỂ LƯU VÀ HIỂN THỊ LẠI DỮ LIỆU ĐÃ NHẬP KHI CÓ LỖI
        public string TempFullName { get; set; } = string.Empty;
        public string TempEmail { get; set; } = string.Empty;
        public string TempPhone { get; set; } = string.Empty;
        public string TempAddress { get; set; } = string.Empty;
        public string TempPaymentMethod { get; set; } = "COD";
        public List<UserAddress>? UserAddresses { get; set; }
    


    // ⚡ Thông tin thanh toán
    public decimal ShippingFee { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public string? DiscountCode { get; set; }
    public string? Province { get; set; } // Ví dụ: "Hà Nội"
    public decimal FinalTotal => GrandTotal + ShippingFee - DiscountAmount;
    }
}
