using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MYMVCAPP.Models;
using MYMVCAPP.Models.ViewModels;
using MYMVCAPP.Repository;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace MYMVCAPP.Controllers
{
    public class CartController : Controller
    {
        private readonly DataContext _dataContext;
        private const string CartSessionKey = "Cart"; // 🔑 Key lưu giỏ hàng trong Session

        public CartController(DataContext context)
        {
            _dataContext = context;
        }

        // ========================= 🛒 GIỎ HÀNG =========================

        public IActionResult Index()
        {
            var cartVM = HttpContext.Session.GetObjectFromJson<CartItemViewModel>(CartSessionKey)
                         ?? new CartItemViewModel();

            cartVM.CartItems ??= new List<CartItemModel>();
            cartVM.GrandTotal = cartVM.CartItems.Sum(x => x.Quantity * x.Price);

            return View(cartVM);
        }

        // ========================= 💳 THANH TOÁN (GET) =========================

        public IActionResult Checkout()
        {
            var cartVM = HttpContext.Session.GetObjectFromJson<CartItemViewModel>(CartSessionKey)
                         ?? new CartItemViewModel();

            cartVM.CartItems ??= new List<CartItemModel>();
            cartVM.GrandTotal = cartVM.CartItems.Sum(x => x.Quantity * x.Price);

            return View("~/Views/Checkout/Index.cshtml", cartVM);
        }

        // ========================= 💳 THANH TOÁN (POST) =========================

        [HttpPost]
        public IActionResult Checkout(CartItemViewModel model)
        {
            var cartVM = HttpContext.Session.GetObjectFromJson<CartItemViewModel>(CartSessionKey)
                         ?? new CartItemViewModel();

            cartVM.CartItems ??= new List<CartItemModel>();
            cartVM.GrandTotal = cartVM.CartItems.Sum(x => x.Quantity * x.Price);

            // ✅ 1. Tính phí vận chuyển
            cartVM.Province = model.Province;
            if (!string.IsNullOrEmpty(cartVM.Province))
            {
                cartVM.ShippingFee = cartVM.Province.Contains("Hà Nội", StringComparison.OrdinalIgnoreCase)
                    ? 15000
                    : 30000;
            }

            // ✅ 2. Áp dụng mã giảm giá
            cartVM.DiscountCode = model.DiscountCode?.Trim().ToUpper();

            if (cartVM.DiscountCode == "FREESHIP")
            {
                cartVM.DiscountAmount = cartVM.ShippingFee;
            }
            else if (cartVM.DiscountCode == "SALE10")
            {
                cartVM.DiscountAmount = cartVM.GrandTotal * 0.1m;
            }
            else
            {
                cartVM.DiscountAmount = 0;
            }

            // ✅ Lưu session lại
            HttpContext.Session.SetObjectAsJson(CartSessionKey, cartVM);

            TempData["SuccessMessage"] = "Đã áp dụng thông tin thanh toán.";

            return View("~/Views/Checkout/Index.cshtml", cartVM);
        }

        // ========================= ➕ THÊM SẢN PHẨM =========================

        [HttpGet]
        public async Task<IActionResult> Add(int? productId, int? variantId)
        {
            if (productId == null && variantId == null)
                return BadRequest("Thiếu thông tin sản phẩm.");

            ProductModel product;
            ProductVariant variant = null;
            decimal price;
            string option = "";

            if (variantId != null)
            {
                variant = await _dataContext.ProductVariants
                            .Include(v => v.Product)
                            .FirstOrDefaultAsync(v => v.Id == variantId);

                if (variant == null) return NotFound();

                product = variant.Product;
                price = variant.Price;
                option = !string.IsNullOrEmpty(variant.Color) ? variant.Color :
                         (!string.IsNullOrEmpty(variant.Weight) ? variant.Weight : "");
            }
            else
            {
                product = await _dataContext.Products.FindAsync(productId);
                if (product == null) return NotFound();
                price = product.Price;
            }

            var cartVM = HttpContext.Session.GetObjectFromJson<CartItemViewModel>(CartSessionKey) ?? new CartItemViewModel();
            cartVM.CartItems ??= new List<CartItemModel>();

            var item = cartVM.CartItems.FirstOrDefault(c =>
                c.ProductId == product.Id && (c.VariantId ?? 0) == (variantId ?? 0) && (c.Option ?? "") == option);

            if (item == null)
            {
                cartVM.CartItems.Add(new CartItemModel
                {
                    ProductId = product.Id,
                    VariantId = variantId,
                    ProductName = product.Name,
                    Img = product.Img,
                    Quantity = 1,
                    Price = price,
                    Option = option
                });
            }
            else
            {
                item.Quantity += 1;
            }

            cartVM.GrandTotal = cartVM.CartItems.Sum(x => x.Quantity * x.Price);
            HttpContext.Session.SetObjectAsJson(CartSessionKey, cartVM);

            TempData["SuccessMessage"] = $"Đã thêm sản phẩm \"{product.Name}\" {(!string.IsNullOrEmpty(option) ? $"({option})" : "")} vào giỏ hàng!";

            var referer = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(referer))
                return Redirect(referer);

            return RedirectToAction("Index");
        }

        // ========================= 🔼 TĂNG SỐ LƯỢNG =========================

        [HttpGet]
        public IActionResult Increase(int productId, int? variantId)
        {
            var cartVM = HttpContext.Session.GetObjectFromJson<CartItemViewModel>(CartSessionKey)
                         ?? new CartItemViewModel();

            cartVM.CartItems ??= new List<CartItemModel>();

            var item = cartVM.CartItems.FirstOrDefault(c =>
                c.ProductId == productId && (c.VariantId ?? 0) == (variantId ?? 0));

            if (item != null)
            {
                item.Quantity += 1;
                TempData["InfoMessage"] = $"Đã tăng số lượng sản phẩm \"{item.ProductName}\".";
            }

            cartVM.GrandTotal = cartVM.CartItems.Sum(x => x.Quantity * x.Price);
            HttpContext.Session.SetObjectAsJson(CartSessionKey, cartVM);

            return Redirect(Request.Headers["Referer"].ToString() ?? "/Cart/Index");
        }

        // ========================= 🔽 GIẢM SỐ LƯỢNG =========================

        [HttpGet]
        public IActionResult Decrease(int productId, int? variantId)
        {
            var cartVM = HttpContext.Session.GetObjectFromJson<CartItemViewModel>(CartSessionKey)
                         ?? new CartItemViewModel();

            cartVM.CartItems ??= new List<CartItemModel>();

            var item = cartVM.CartItems.FirstOrDefault(c =>
                c.ProductId == productId && (c.VariantId ?? 0) == (variantId ?? 0));

            if (item != null)
            {
                item.Quantity -= 1;
                if (item.Quantity <= 0)
                {
                    cartVM.CartItems.Remove(item);
                    TempData["InfoMessage"] = $"Đã xóa \"{item.ProductName}\" khỏi giỏ hàng.";
                }
                else
                {
                    TempData["InfoMessage"] = $"Đã giảm số lượng \"{item.ProductName}\".";
                }
            }

            cartVM.GrandTotal = cartVM.CartItems.Sum(x => x.Quantity * x.Price);
            HttpContext.Session.SetObjectAsJson(CartSessionKey, cartVM);

            return Redirect(Request.Headers["Referer"].ToString() ?? "/Cart/Index");
        }

        // ========================= ❌ XÓA SẢN PHẨM =========================

        [HttpGet]
        public IActionResult Remove(int productId, int? variantId)
        {
            var cartVM = HttpContext.Session.GetObjectFromJson<CartItemViewModel>(CartSessionKey)
                         ?? new CartItemViewModel();

            cartVM.CartItems ??= new List<CartItemModel>();

            var item = cartVM.CartItems.FirstOrDefault(c =>
                c.ProductId == productId && (c.VariantId ?? 0) == (variantId ?? 0));

            if (item != null)
            {
                cartVM.CartItems.Remove(item);
                TempData["InfoMessage"] = $"Đã xóa sản phẩm \"{item.ProductName}\" khỏi giỏ hàng.";
            }

            cartVM.GrandTotal = cartVM.CartItems.Sum(x => x.Quantity * x.Price);
            HttpContext.Session.SetObjectAsJson(CartSessionKey, cartVM);

            return Redirect(Request.Headers["Referer"].ToString() ?? "/Cart/Index");
        }

        // ========================= 🟡 XỬ LÝ CHECKBOX (XÓA / MUA CHỌN) =========================

        [HttpPost]
        public IActionResult ProcessSelected(List<int> selectedIds, string actionType)
        {
            var cartVM = HttpContext.Session.GetObjectFromJson<CartItemViewModel>(CartSessionKey)
                         ?? new CartItemViewModel();

            cartVM.CartItems ??= new List<CartItemModel>();

            if (selectedIds == null || !selectedIds.Any())
            {
                TempData["ErrorMessage"] = "Vui lòng chọn ít nhất một sản phẩm.";
                return RedirectToAction("Index");
            }

            if (actionType == "delete")
            {
                cartVM.CartItems.RemoveAll(item => selectedIds.Contains(item.VariantId ?? item.ProductId));
                cartVM.GrandTotal = cartVM.CartItems.Sum(x => x.Quantity * x.Price);
                HttpContext.Session.SetObjectAsJson(CartSessionKey, cartVM);
                TempData["InfoMessage"] = "Đã xóa các sản phẩm đã chọn khỏi giỏ hàng.";
            }
            else if (actionType == "buy")
            {
                var selectedItems = cartVM.CartItems.Where(item => selectedIds.Contains(item.VariantId ?? item.ProductId)).ToList();

                var newCartVM = new CartItemViewModel
                {
                    CartItems = selectedItems,
                    GrandTotal = selectedItems.Sum(x => x.Quantity * x.Price)
                };

                HttpContext.Session.SetObjectAsJson(CartSessionKey, newCartVM);
                TempData["SuccessMessage"] = "Đã chuyển tới trang thanh toán.";
                return RedirectToAction("Checkout", "Cart");
            }

            return RedirectToAction("Index");
        }
    }
}
