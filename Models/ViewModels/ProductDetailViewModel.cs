namespace MYMVCAPP.Models.ViewModels
{
    public class ProductDetailViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Img { get; set; }
        public string Description { get; set; }
         public ProductModel Product { get; set; }
        public IEnumerable<ProductVariant> Variants { get; set; }
        public List<ProductModel> RecommendedProducts { get; set; }
    }
}
