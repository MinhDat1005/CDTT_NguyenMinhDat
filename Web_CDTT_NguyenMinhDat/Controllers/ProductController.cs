using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_CDTT_NguyenMinhDat.Models;
using Web_CDTT_NguyenMinhDat.Models.ViewModels;
using Web_CDTT_NguyenMinhDat.Repository;

namespace Web_CDTT_NguyenMinhDat.Controllers
{
    public class ProductController : Controller
    {
        private readonly DataContext _dataContext;
        public ProductController (DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public async Task<IActionResult> Index(string sort_by = "", string startprice = "", string endprice = "")
        {
            // Lấy danh sách sản phẩm từ cơ sở dữ liệu
            var products = _dataContext.Products.AsQueryable();

            // Lọc theo giá trị sort_by
            if (sort_by == "price_increase")
            {
                products = products.OrderBy(p => p.Price);
            }
            else if (sort_by == "price_decrease")
            {
                products = products.OrderByDescending(p => p.Price);
            }
            else if (sort_by == "price_newest")
            {
                products = products.OrderByDescending(p => p.Id);
            }
            else if (sort_by == "price_oldest")
            {
                products = products.OrderBy(p => p.Id);
            }

            // Lọc theo giá nếu có giá tối thiểu và tối đa
            if (!string.IsNullOrEmpty(startprice) && !string.IsNullOrEmpty(endprice))
            {
                decimal startPriceValue;
                decimal endPriceValue;

                // Kiểm tra và phân tích giá trị
                if (decimal.TryParse(startprice, out startPriceValue) && decimal.TryParse(endprice, out endPriceValue))
                {
                    products = products.Where(p => p.Price >= startPriceValue && p.Price <= endPriceValue);
                }
            }

            // Tạo ViewModel
            var viewModel = new ProductFilterViewModel
            {
                Products = await products.ToListAsync(),
                Categories = await _dataContext.Categories.ToListAsync(),
                Brands = await _dataContext.Brands.ToListAsync()
            };

            return View(viewModel);
        }






        //public IActionResult Details(int id)
        //{
        //	var product = _dataContext.Products.Include(p => p.Category).Include(p => p.Brand).FirstOrDefault(p => p.Id == id);
        //	if (product == null)
        //	{
        //		return RedirectToAction("Index");
        //	}
        //	//related product

        //	return View(product);
        //}
        public async Task<IActionResult> Details(int? id)
		{
			if (id == null)
			{
				return RedirectToAction("Index");
			}

			var product = await _dataContext.Products
				.Include(p => p.Category).Include(p => p.RatingDetails)
				.Include(p => p.Brand)
				.FirstOrDefaultAsync(p => p.Id == id);

			if (product == null)
			{
				return RedirectToAction("Index");
			}

			// Lấy danh sách sản phẩm liên quan
			var relatedProducts = await _dataContext.Products
				.Where(p => p.CategoryId == product.CategoryId && p.Id != product.Id)
				.Take(4)
				.ToListAsync();
			ViewBag.RelatedProducts = relatedProducts;

			var viewModel = new ProductDetailViewModel
			{
				ProductDetail = product,
			};
			return View(viewModel);
		}


		public async Task <IActionResult> Search(string searchTerm)
        {
            var products = await _dataContext.Products
                .Where(p=>p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm))
                .ToListAsync(); ;
            ViewBag.Keyword = searchTerm;
            return View(products);
        }

        //đánh giá sản phẩm
		public async Task <IActionResult> CommentProduct(RatingModel rating)
		{
			if(ModelState.IsValid)
			{
				var rantingEntity = new RatingModel
				{
					ProductId = rating.ProductId,
					Name = rating.Name,
					Comment = rating.Comment,
					Email = rating.Email,
					Star = rating.Star
				};

				_dataContext.Ratings.Add(rantingEntity);
				await _dataContext.SaveChangesAsync();
				TempData["Message"] = "Đánh giá thành công.";
				return Redirect(Request.Headers["Referer"]);
			}
			else
			{
				TempData["Message"] = "Model có một vài thứ đang bị lỗi";
				List<string> errors = new List<string>();
				foreach (var value in ModelState.Values)
				{
					foreach (var error in value.Errors)
					{
						errors.Add(error.ErrorMessage);
					}
				}
				string errorMessage = string.Join("\n", errors);
				return RedirectToAction("Details", new
				{
					id = rating.ProductId
				});
			}
			return Redirect(Request.Headers["Referer"]);
		}

        //
       

    }
}
