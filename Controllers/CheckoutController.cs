using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MYMVCAPP.Models;
using MYMVCAPP.Models.ViewModels;
using MYMVCAPP.Repository;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace MYMVCAPP.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly DataContext _context;
        private readonly ILogger<CheckoutController> _logger;
        private const string CheckoutSessionKey = "CheckoutInfo";

        public CheckoutController(DataContext context, ILogger<CheckoutController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // 🛒 Hiển thị trang thanh toán
        public IActionResult Index()
        {
            var cartVM = HttpContext.Session.GetJson<CartItemViewModel>("Cart")
                         ?? new CartItemViewModel
                         {
                             CartItems = new List<CartItemModel>(),
                             GrandTotal = 0
                         };

            // Lấy thông tin địa chỉ tạm lưu từ session
            var addressJson = HttpContext.Session.GetString(CheckoutSessionKey);
            if (!string.IsNullOrEmpty(addressJson))
            {
                try
                {
                    var addr = JsonSerializer.Deserialize<Dictionary<string, string>>(addressJson);
                    cartVM.TempFullName = addr.GetValueOrDefault("TempFullName");
                    cartVM.TempEmail = addr.GetValueOrDefault("TempEmail");
                    cartVM.TempPhone = addr.GetValueOrDefault("TempPhone");
                    cartVM.TempAddress = addr.GetValueOrDefault("TempAddress");
                    cartVM.Province = addr.GetValueOrDefault("Province");
                    cartVM.TempPaymentMethod = addr.GetValueOrDefault("TempPaymentMethod");
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Lỗi khi đọc session địa chỉ Checkout");
                }
            }

            // ✅ SỬA: Tính phí vận chuyển (dùng so sánh không phân biệt hoa/thường)
            if (string.Equals(cartVM.Province, "Hà Nội", StringComparison.OrdinalIgnoreCase))
                cartVM.ShippingFee = 15000;
            else if (!string.IsNullOrEmpty(cartVM.Province))
                cartVM.ShippingFee = 30000;
            else
                cartVM.ShippingFee = 0;

            return View(cartVM);
        }

        // 💾 Lưu địa chỉ giao hàng
        [HttpPost]
        [ValidateAntiForgeryToken] // ✅ SỬA: Thêm bảo mật
        public async Task<IActionResult> SaveAddress(CartItemViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.TempAddress))
            {
                TempData["Error"] = "Vui lòng nhập địa chỉ giao hàng";
                return RedirectToAction("Index");
            }

            try
            {
                // Lưu vào session
                var addressData = new Dictionary<string, string>
                {
                    ["TempFullName"] = model.TempFullName ?? "",
                    ["TempEmail"] = model.TempEmail ?? "",
                    ["TempPhone"] = model.TempPhone ?? "",
                    ["TempAddress"] = model.TempAddress ?? "",
                    ["Province"] = model.Province ?? "",
                    ["TempPaymentMethod"] = model.TempPaymentMethod ?? ""
                };
                HttpContext.Session.SetString(CheckoutSessionKey, JsonSerializer.Serialize(addressData));

                // Nếu người dùng đã đăng nhập thì cập nhật DB
                var userId = HttpContext.Session.GetInt32("UserId");
                if (userId != null)
                {
                    // ✅ SỬA: Chuyển sang async
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                    if (user != null)
                    {
                        user.FullName = model.TempFullName;
                        user.Email = model.TempEmail;
                        user.Phone = model.TempPhone;
                        user.Address = model.TempAddress;
                        user.Province = model.Province;
                        _context.Update(user);
                        await _context.SaveChangesAsync(); // ✅ SỬA: Chuyển sang async
                    }
                }
                TempData["SuccessMessage"] = "Đã lưu địa chỉ giao hàng thành công.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lưu địa chỉ");
                TempData["Error"] = "Lỗi khi lưu địa chỉ, vui lòng thử lại.";
            }
            
            return RedirectToAction("Index");
        }

        // ✅ Đặt hàng
        [HttpPost]
        [ValidateAntiForgeryToken]
        // ✅ SỬA: Thêm "string Province" để khớp với form
        public async Task<IActionResult> PlaceOrder(string FullName, string Email, string Phone, string Address, string Province, string PaymentMethod)
        {
            var cartVM = HttpContext.Session.GetJson<CartItemViewModel>("Cart");

            if (cartVM == null || cartVM.CartItems == null || !cartVM.CartItems.Any())
            {
                TempData["Error"] = "Giỏ hàng trống, không thể đặt hàng.";
                return RedirectToAction("Index", "Cart");
            }
            
            if (string.IsNullOrWhiteSpace(Address))
            {
                TempData["Error"] = "Vui lòng nhập địa chỉ giao hàng.";
                return RedirectToAction("Index");
            }
            
            // ✅ SỬA: Lấy phí ship từ tính toán động (nếu có) hoặc tính lại cho chắc chắn
            decimal shippingFee = 0;
            if (string.Equals(Province, "Hà Nội", StringComparison.OrdinalIgnoreCase))
                shippingFee = 15000;
            else if (!string.IsNullOrEmpty(Province))
                shippingFee = 30000;

            cartVM.ShippingFee = shippingFee; // Cập nhật phí ship cuối cùng


            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var order = new Order
                {
                    FullName = FullName.Trim(),
                    Email = Email?.Trim() ?? "",
                    Phone = Phone.Trim(),
                    Address = Address.Trim(),
                    // Province = Province?.Trim() ?? "", // ✅ SỬA: Lưu Province
                    PaymentMethod = PaymentMethod ?? "COD",
                    ShippingFee = cartVM.ShippingFee, // Dùng phí ship đã tính
                    TotalAmount = cartVM.GrandTotal + cartVM.ShippingFee - cartVM.DiscountAmount,
                    CreatedAt = DateTime.UtcNow,
                    Status = "Pending"
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync(); // Lưu để lấy OrderId

                // ✅ SỬA: TỐI ƯU HÓA N+1 QUERY (Rất quan trọng)
                var variantIds = cartVM.CartItems
                    .Where(i => i.VariantId.HasValue)
                    .Select(i => i.VariantId.Value)
                    .Distinct()
                    .ToList();
                
                var productIdsForFallback = cartVM.CartItems
                    .Where(i => !i.VariantId.HasValue)
                    .Select(i => i.ProductId)
                    .Distinct()
                    .ToList();

                var variantsById = await _context.ProductVariants
                    .Where(v => variantIds.Contains(v.Id))
                    .ToDictionaryAsync(v => v.Id);
                
                var variantsByProductId = await _context.ProductVariants
                    .Where(v => productIdsForFallback.Contains(v.ProductId))
                    .GroupBy(v => v.ProductId)
                    .Select(g => g.First()) 
                    .ToDictionaryAsync(v => v.ProductId);
                // KẾT THÚC TỐI ƯU HÓA

                foreach (var item in cartVM.CartItems)
                {
                    _context.OrderItems.Add(new OrderItem
                    {
                        OrderId = order.Id,
                        ProductId = item.ProductId,
                        ProductName = item.ProductName,
                        Quantity = item.Quantity,
                        UnitPrice = item.Price
                    });

                    // Cập nhật tồn kho từ dictionary (không truy vấn CSDL trong loop)
                    ProductVariant variantToUpdate = null;
                    if (item.VariantId.HasValue)
                    {
                        variantsById.TryGetValue(item.VariantId.Value, out variantToUpdate);
                    }
                    else
                    {
                        variantsByProductId.TryGetValue(item.ProductId, out variantToUpdate);
                    }

                    if (variantToUpdate != null)
                    {
                        variantToUpdate.Stock = Math.Max(0, variantToUpdate.Stock - item.Quantity);
                        _context.ProductVariants.Update(variantToUpdate); 
                    }
                }

                await _context.SaveChangesAsync(); // Lưu OrderItems và cập nhật Stock
                await transaction.CommitAsync();

                HttpContext.Session.Remove("Cart");
                HttpContext.Session.Remove(CheckoutSessionKey); // ✅ SỬA: Xóa cả session địa chỉ

                TempData["Success"] = $"Đặt hàng thành công! Mã đơn hàng: {order.Id}";
                return RedirectToAction("OrderSuccess", new { id = order.Id });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "❌ Lỗi khi lưu đơn hàng");
                TempData["Error"] = "Có lỗi xảy ra khi đặt hàng. Vui lòng thử lại.";
                return RedirectToAction("Index");
            }
        }

        // 🧾 Trang hiển thị khi đặt hàng thành công
        public async Task<IActionResult> OrderSuccess(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound();

            return View(order);
        }

        // -----------------------------------------------------------------
        // ✅ MỚI: Action để xử lý AJAX tính phí vận chuyển
        // -----------------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CalculateShipping(string province, decimal grandTotal, decimal discountAmount)
        {
            try
            {
                decimal shippingFee = 0;

                // Logic tính phí phải đồng bộ với logic trong Action 'Index'
                if (string.Equals(province, "Hà Nội", StringComparison.OrdinalIgnoreCase))
                {
                    shippingFee = 15000;
                }
                else if (!string.IsNullOrEmpty(province)) // Bao gồm "TP.HCM", "Đà Nẵng", "Khác"
                {
                    shippingFee = 30000;
                }
                // Nếu province là "" (Chưa chọn), shippingFee sẽ là 0

                decimal finalTotal = grandTotal + shippingFee - discountAmount;

                // Trả về kết quả dạng JSON
                return Json(new { shippingFee = shippingFee, finalTotal = finalTotal });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tính phí vận chuyển AJAX");
                // Trả về lỗi
                return BadRequest(new { error = "Không thể tính phí vận chuyển." });
            }
        }
    }
}