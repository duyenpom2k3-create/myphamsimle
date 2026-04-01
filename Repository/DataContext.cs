using Microsoft.EntityFrameworkCore;
using MYMVCAPP.Models;

namespace MYMVCAPP.Repository
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        // Các DbSet (bảng)
        public DbSet<BrandModel> Brands { get; set; }
        public DbSet<ProductModel> Products { get; set; }
        public DbSet<UserModel> Users { get; set; }
        public DbSet<BannerModel> Banners { get; set; }
        public DbSet<MenuModel> Menus { get; set; }
        public DbSet<MenuUserModel> MenuUsers { get; set; }
        public DbSet<CategoryModel> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<FooterItem> FooterItems { get; set; }
        public DbSet<UserAddress> UserAddresses { get; set; }
        public DbSet<WishlistModel> Wishlist { get; set; }
        public DbSet<FAQModel> FAQs { get; set; }
        public DbSet<HeaderModel> Headers { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ⚙️ Cấu hình mối quan hệ Category ↔ Menu
            modelBuilder.Entity<CategoryModel>()
                .HasOne(c => c.Menu)
                .WithMany(m => m.Categories)
                .HasForeignKey(c => c.MenuId)
                .OnDelete(DeleteBehavior.Cascade);

            // ⚙️ Cấu hình độ chính xác cho cột tiền tệ (DECIMAL)
            
            // 1. ProductModel.Price (Giải quyết cảnh báo Decimal)
            modelBuilder.Entity<ProductModel>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");
            
            // 2. Order.TotalAmount
            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasColumnType("decimal(18,2)");

            // 3. OrderItem.UnitPrice
            modelBuilder.Entity<OrderItem>()
                .Property(i => i.UnitPrice)
                .HasColumnType("decimal(18,2)");

            // ⚙️ Cấu hình Order và OrderItem (Mối quan hệ 1-N)
            // LƯU Ý: Khai báo này là hoàn toàn chính xác và tuân thủ quy ước:
            modelBuilder.Entity<OrderItem>()
                .HasOne(o => o.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(o => o.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
                
            // (Không cần phải khai báo .WithOne(i => i.Order) vì bạn đã xóa [ForeignKey]
            // trong OrderItem.cs ở bước trước, giúp EF Core tránh nhầm lẫn OrderId1.)
 modelBuilder.Entity<WishlistModel>()
                .HasOne(w => w.User)
                .WithMany(u => u.Wishlists)
                .HasForeignKey(w => w.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<WishlistModel>()
                .HasOne(w => w.Product)
                .WithMany(p => p.Wishlists)
                .HasForeignKey(w => w.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}