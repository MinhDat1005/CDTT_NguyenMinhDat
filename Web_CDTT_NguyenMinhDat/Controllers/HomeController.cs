using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
using Web_CDTT_NguyenMinhDat.Models;
using Web_CDTT_NguyenMinhDat.Repository;

namespace Web_CDTT_NguyenMinhDat.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DataContext _dataContext;
        private readonly UserManager<AppUserModel> _userManager;

        public HomeController(ILogger<HomeController> logger, DataContext dataContext, UserManager<AppUserModel> userManager)
        {
            _logger = logger;
            _dataContext = dataContext;
            _userManager = userManager;
        }

        public IActionResult Index()
        {

            var allproduct = _dataContext.Products.Include("Category").ToList();
            return View(allproduct);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]


        //Error
        public IActionResult Error(int statuscode)
        {
            
                if (statuscode == 404)
                {
                    return View("NotFound");
                }
                else
                {
                return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }

        }
        //Contact
        public IActionResult Contact()
        {
            var contact = _dataContext.Contacts.ToList();

            return View(contact);
        }

        //Yêu thích
        [HttpPost]
		public async Task <IActionResult> AddWishlist(int Id, WishListModel wishList)
		{
			var user = await _userManager.GetUserAsync(User);
            var wishListProduct = new WishListModel
            {
				ProductId = Id,
				UserId = user.Id
			}
          ;
            _dataContext.WishLists.Add(wishListProduct);

            try
            {
                await _dataContext.SaveChangesAsync();
                return Ok(new { success = true, message = "Add to wishlist Successfully" });
            }
            catch (Exception ex) { return StatusCode(500, "An error occurred while updating the wishlist status"); }

			return View();
		}

		//So sánh
		[HttpPost]
		public async Task<IActionResult> AddCompare(int Id, CompareModel compareModel)
		{
			var user = await _userManager.GetUserAsync(User);
			var compareProduct = new CompareModel
			{
				ProductId = Id,
				UserId = user.Id
			}
		  ;
			_dataContext.Compares.Add(compareProduct);

			try
			{
				await _dataContext.SaveChangesAsync();
				return Ok(new { success = true, message = "Add to compare Successfully" });
			}
			catch (Exception ex) { return StatusCode(500, "An error occurred while updating the order status"); }

			return View();
		}

		public async Task<IActionResult> Compare()
		{
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var compare_product = await (from c in _dataContext.Compares
										 join p in _dataContext.Products on c.ProductId equals p.Id
										 join u in _dataContext.Users on c.UserId equals u.Id
                                         where c.UserId == userId
                                         select new
										 {
											 User = u,
											 Product = p,
											 Compare = c,
                                             Id = c.Id // Adding this explicitly
                                         }).ToListAsync();
			return View(compare_product);
		}


        public async Task<IActionResult> Wishlist()
        {
            // Get the logged-in user's ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var wish_product = await (from w in _dataContext.WishLists
                                      join p in _dataContext.Products on w.ProductId equals p.Id
                                      join u in _dataContext.Users on w.UserId equals u.Id
                                      where w.UserId == userId // Filter by the logged-in user's ID
                                      select new
                                      {
                                          User = u,
                                          Product = p,
                                          Wishlists = w,
                                          Id = w.Id 
                                      }).ToListAsync();

            return View(wish_product);
        }


		public async Task<IActionResult> DeleteCompare(int Id)
		{
			if (Id <= 0)
			{
				TempData["Error"] = "Invalid ID.";
				return RedirectToAction("Index");
			}

			var compareProduct = await _dataContext.Compares.FindAsync(Id);

			if (compareProduct == null)
			{
				TempData["Error"] = "Product not found.";
				return RedirectToAction("Compare"); 
			}

			_dataContext.Compares.Remove(compareProduct);
			await _dataContext.SaveChangesAsync(); 

			TempData["Message"] = "Xóa thành công!"; 
			return RedirectToAction("Compare"); 
		}


		public async Task<IActionResult> DeleteWish(int Id)
		{
			if (Id <= 0)
			{
				TempData["Error"] = "Invalid ID.";
				return RedirectToAction("Index");
			}

			var wishProduct = await _dataContext.WishLists.FindAsync(Id);

			if (wishProduct == null)
			{
				TempData["Error"] = "Product not found.";
				return RedirectToAction("Wishlist");
			}

			_dataContext.WishLists.Remove(wishProduct);
			await _dataContext.SaveChangesAsync();

			TempData["Message"] = "Xóa thành công!";
			return RedirectToAction("Wishlist");
		}


	}
}
