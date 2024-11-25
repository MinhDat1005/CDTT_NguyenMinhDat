using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Web_CDTT_NguyenMinhDat.Models;
using Web_CDTT_NguyenMinhDat.Models.ViewModels;
using Web_CDTT_NguyenMinhDat.Repository;

namespace Web_CDTT_NguyenMinhDat.Controllers
{
	public class CartController : Controller
	{
		private readonly DataContext _dataContext;

		public CartController(DataContext dataContext) { 
		_dataContext = dataContext;
		}
        public IActionResult Index()
        {
            List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            var shippingPriceCookie = Request.Cookies["ShippingPrice"];
            decimal shippingPrice = 0;

            if (shippingPriceCookie != null)
            {
                var shippingPriceJson = shippingPriceCookie;
                shippingPrice = JsonConvert.DeserializeObject<decimal>(shippingPriceJson);
            }

            // Nhận coupon 
            var coupon_code = Request.Cookies["CouponTitle"];
            var priceCouponCookie = Request.Cookies["PriceCoupon"]; // Lấy giá trị giảm giá từ cookie
            decimal discountPrice = 0;

            if (!string.IsNullOrEmpty(priceCouponCookie))
            {
                discountPrice = JsonConvert.DeserializeObject<decimal>(priceCouponCookie); // Chuyển đổi giá trị từ cookie sang decimal
            }

            CartItemVM cartItemVM = new()
            {
                CartItems = cartItems,
                GrandTotal = cartItems.Sum(x => x.Quantity * x.Price) + shippingPrice - discountPrice, // Tính tổng với giá trị giảm giá
                ShippingCOD = shippingPrice,
                CouponCOD = coupon_code
            };
            return View(cartItemVM);
        }


        //Thêm giỏ hàng
        public async Task<IActionResult> AddCart(int Id)
		{
			
			ProductModel product = await _dataContext.Products.FindAsync(Id);
			if (product == null)
			{
				return NotFound($"Product with ID {Id} not found.");
			}
			List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
			CartItemModel cartItem = cart.FirstOrDefault(c => c.ProductId == Id);
			if (cartItem == null)
			{
				
				cart.Add(new CartItemModel(product));
			}
			else
			{
				
				cartItem.Quantity += 1;
			}
			TempData["Message"] = "Thêm sản phẩm vào giỏ hàng thành công!";
			HttpContext.Session.SetJson("Cart", cart);
			var referer = Request.Headers["Referer"].ToString();
			if (string.IsNullOrEmpty(referer))
			{
				return RedirectToAction("Index", "Home");
			}
			return Redirect(referer);
		}


		//giamsoluong
		public async Task<IActionResult> Decrease (int Id)
		{
			List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");
			CartItemModel cartItems = cart.FirstOrDefault(c => c.ProductId == Id);
			if (cartItems.Quantity > 1) {
				--cartItems.Quantity;
					}
			else
			{
				cart.RemoveAll(p=>p.ProductId == Id);
			}
			if (cart.Count == 0)
			{

				HttpContext.Session.Remove("Cart");
			}
			else {
				HttpContext.Session.SetJson("Cart", cart);
			}
			TempData["Message"] = "Giảm số lượng sản phẩm thành công!";

			return RedirectToAction("Index");
		}
		//Tăng
		public async Task<IActionResult> Increase(int Id)
		{
			ProductModel product = _dataContext.Products.Where(p=>p.Id == Id).FirstOrDefault();
			List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");

			CartItemModel cartItems = cart.FirstOrDefault(c => c.ProductId == Id);

			if (cartItems.Quantity >=1 && product.Quantity >cartItems.Quantity)
			{
				++cartItems.Quantity;
				TempData["success"] = "Tăng số lượng sản phẩm thành công!";
			}
            else
            {
                cartItems.Quantity = product.Quantity;
				TempData["Message"] = "Số lượng sản phẩm đã hết.";
			}

            HttpContext.Session.SetJson("Cart", cart);
			

			return RedirectToAction("Index");
		}

		//Xóa
		public async Task<IActionResult> Remove(int Id)
		{
			List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");

			cart.RemoveAll(p => p.ProductId == Id);
			if (cart.Count == 0)
			{
				HttpContext.Session.Remove("Cart");

			}
			else
			{
				HttpContext.Session.SetJson("Cart", cart);
			}
			TempData["Message"] = "Xóa sản phẩm thành công!";

			return RedirectToAction("Index");
		}

		//Clear gio hang
		public async Task<IActionResult> Clear()
		{
			
			HttpContext.Session.Remove("Cart");
			TempData["Message"] = "Xóa toàn bộ giỏ hàng thành công!";


			return RedirectToAction("Index");
		}
		
		//Shipping
		[HttpPost]
		[Route("Cart/GetShipping")]

		public async Task <IActionResult > GetShipping(ShippingModel shippingModel, string quan, string phuong, string tinh)
		{
			var existShiping= await _dataContext.Shippings.FirstOrDefaultAsync(x=>x.City==tinh && x.District==quan && x.Ward==phuong);
			decimal ShippingPrice = 0;
			if (existShiping != null) {
				ShippingPrice = existShiping.Price;

			}
			else { 
				ShippingPrice = 50000;
			}

			var ShippingPriceJson = JsonConvert.SerializeObject(ShippingPrice);
			try {
				var cookieOptions = new CookieOptions
				{
					HttpOnly = true,
					Expires = DateTimeOffset.UtcNow.AddMinutes(30),
					Secure = true,
				};
				Response.Cookies.Append("ShippingPrice", ShippingPriceJson, cookieOptions);
			} catch (Exception ex) { 
				Console.WriteLine($"Error adding shipping price cookie:{ex.Message}");
			}
			return Json(new {ShippingPrice });
		}
		[HttpGet]
		[Route("Cart/DeleteShipping")]
		public IActionResult DeleteShipping()
		{
			Response.Cookies.Delete("ShippingPrice");
			//return Json(new { succes = true });
			return RedirectToAction("Index", "Cart");
		}

		//Get Coupon
		

        [HttpPost]
        [Route("Cart/GetCoupon")]
        public async Task<IActionResult> GetCoupon(CouponModel couponModel, string coupon_value)
        {
            var validCoupon = await _dataContext.Coupons.FirstOrDefaultAsync(c => c.Name == coupon_value);
            if (validCoupon != null)
            {
                string couponTitle = validCoupon.Name + " - " + validCoupon.Description;
                TimeSpan remainingTime = validCoupon.CreatedExpired - DateTime.Now;
                int daysRemaining = remainingTime.Days;

                if (daysRemaining >= 0)
                {
                    try
                    {
                        var cookieOption = new CookieOptions
                        {
                            HttpOnly = true,
                            Expires = DateTimeOffset.UtcNow.AddMinutes(30),
                            Secure = true,
                            SameSite = SameSiteMode.Strict,
                        };
                        Response.Cookies.Append("CouponTitle", couponTitle, cookieOption);
                        Response.Cookies.Append("PriceCoupon", validCoupon.PriceCoupon.ToString(), cookieOption); 
                        return Ok(new { success = true, message = "Coupon applied successfully" });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error adding apply coupon cookie: {ex.Message}");
                        return Ok(new { success = false, message = "Coupon applied failed" });
                    }
                }
                else
                {
                    return Ok(new { success = false, message = "Coupon has expired" });
                }
            }
            else
            {
                return Ok(new { success = false, message = "Coupon not existed" });
            }
        }
    }
}
