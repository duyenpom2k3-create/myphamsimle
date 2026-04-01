namespace MYMVCAPP.Models
{
    public class CartItemModel
    {
        public int ProductId { get; set; }
    public string ProductName { get; set; }
    public string Img { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string Option { get; set; } // 🔸 Màu / khối lượng

        public decimal Total => Price * Quantity;  // Tổng tiền hàng
   
        public int? VariantId { get; set; } // 🔹 null nếu không có biến thể

        public CartItemModel() {}

        public CartItemModel(ProductModel product)
        {
            ProductId = product.Id;
            ProductName = product.Name;
            Price = product.Price;
            Quantity = 1;
        }

        public CartItemModel(ProductVariant variant)
        {
            ProductId = variant.ProductId;
            VariantId = variant.Id;
            ProductName = variant.Product.Name;
            Price = variant.Price;
            Quantity = 1;
            Option = !string.IsNullOrEmpty(variant.Color) ? variant.Color : variant.Weight;
        }
    }
}
