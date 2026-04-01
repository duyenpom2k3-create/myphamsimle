using Microsoft.EntityFrameworkCore;
using MYMVCAPP.Repository;
using MYMVCAPP.Models;

namespace MYMVCAPP.SeedData
{
    public class SeedData
    {
        public static void SeedingData(DataContext _context)
        {
            _context.Database.Migrate();
            if (!_context.Products.Any())
            {
                CategoryModel apple = new CategoryModel { Name = "Áo", Slug = "TeLab", Description = "giới thiệu", Status = 1 };
                // _context.Products.AddRange(

                //     new ProductModel
                //     { Name = "Áo thun cotton", Slug = "Áo", Description = "Áo thun sản xuất tại Việt Nam", Img = "anh.jpg", Category = Áo, Price = 150.000})
            }
            _context.SaveChanges();
            }
        }

    }