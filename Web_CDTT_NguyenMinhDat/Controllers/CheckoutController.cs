using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;
using Web_CDTT_NguyenMinhDat.Models;
using Web_CDTT_NguyenMinhDat.Repository;
using Web_CDTT_NguyenMinhDat.Repository.SendMail;

namespace Web_CDTT_NguyenMinhDat.Controllers
{
	public class CheckoutController : Controller
	{
		private readonly DataContext _dataContext;
        private readonly IEmailSender _emailSender;

        public CheckoutController (DataContext dataContext,IEmailSender emailSender)
		{
			_dataContext = dataContext;
			_emailSender = emailSender;
		}
		public async Task <IActionResult> Checkout()
		{
			var userEmail = User.FindFirstValue(ClaimTypes.Email);
			if (userEmail == null)
			{
				return RedirectToAction("Login","Account");
			}
			else
			{
				var ordercode = Guid.NewGuid().ToString();
				var orderItem = new OrderModel();
				orderItem.OrderCode = ordercode;
                var shippingPriceCookie = Request.Cookies["ShippingPrice"];
				//-------------------------------------
                decimal shippingPrice = 0;

                if (shippingPriceCookie != null)
                {
                    var shippingPriceJson = shippingPriceCookie;
                    shippingPrice = JsonConvert.DeserializeObject<decimal>(shippingPriceJson);
                }
                orderItem.ShippingCOD = shippingPrice;
				//-----------------------------------------
				var coupon_code = Request.Cookies["CouponTitle"];
                orderItem.CouponCode = coupon_code;
                //-----------------------------------------

                orderItem.CreatedDate = DateTime.Now;
				orderItem.UserName = userEmail;
				orderItem.Status = 1;
				_dataContext.OrderModels.Add(orderItem);
				_dataContext.SaveChanges();
				List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
				foreach (var cart in cartItems) {
					var orderDetails = new OrderDetail();
					orderDetails.Price = cart.Price;
					orderDetails.Quantity = cart.Quantity;
					orderDetails.OrderCode = ordercode;
					orderDetails.ProductId =cart.ProductId;
					orderDetails.UserName = userEmail;

					//upadate quantity
					var product =  await _dataContext.Products.Where(p=>p.Id == cart.ProductId).FirstAsync();
					product.Quantity -= cart.Quantity;
					product.Sold += cart.Quantity;
					_dataContext.Update(product);
					//--------------------------------------------------
					_dataContext.OrderDetails.Add(orderDetails);
					_dataContext.SaveChanges();
				}
				HttpContext.Session.Remove("Cart");

                //sendmail
                var receiver = "datminh100504@gmail.com";
				var subject = "Xác nhận đơn hàng";
				var message = "ELECTRO SHOP, Cảm ơn qúy khách đã mua hàng !";

                await _emailSender.SendEmailAsync(receiver,subject,message, "MDShop", "datminh100504@gmail.com");



                TempData["Message"] = "Checkout thành công !";
				return RedirectToAction("Index","Cart");
			}
			return View();
		}

	}
}
