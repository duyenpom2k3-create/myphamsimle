using Microsoft.EntityFrameworkCore;
using MYMVCAPP.Models;
using MYMVCAPP.Repository;

var builder = WebApplication.CreateBuilder(args);

// ---------------------- DATABASE ----------------------
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectedDb"));
});

// ---------------------- SERVICES ----------------------
builder.Services.AddControllersWithViews();

// ⚙️ Bật cache và session (phải khai báo TRƯỚC khi build app)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // ⏳ thời gian sống của session
    options.Cookie.HttpOnly = true;                 // 🔒 tránh JS truy cập
    options.Cookie.IsEssential = true;              // ⚡ cho phép hoạt động không cần consent
});

// ---------------------- BUILD APP ----------------------
var app = builder.Build();

// ---------------------- MIDDLEWARE ----------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// 👇 THÊM DÒNG NÀY VÀO TRƯỚC app.UseHttpsRedirection();
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ⚡ Bật session ở đây (trước Authorization)
app.UseSession();

app.UseAuthorization();

// ---------------------- ROUTING ----------------------

// Ưu tiên Area route trước
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);

// Các route tuỳ chỉnh
app.MapControllerRoute(
    name: "category",
    pattern: "category/{slug?}",
    defaults: new { controller = "Category", action = "Index" });

app.MapControllerRoute(
    name: "menu",
    pattern: "menu/{slug?}",
    defaults: new { controller = "Menu", action = "Details" });

// Route mặc định — mở trang Login trước
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");
app.MapControllerRoute(
    name: "FooterDetail",
    pattern: "{slug}",
    defaults: new { controller = "Footer", action = "Detail" }
);


// (Nếu muốn khi đăng nhập thành công vào Home, thì redirect ở AccountController)
app.Run();
