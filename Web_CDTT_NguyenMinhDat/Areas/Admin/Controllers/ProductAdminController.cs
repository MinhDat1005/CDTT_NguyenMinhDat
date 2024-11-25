using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Web_CDTT_NguyenMinhDat.Models;
using Web_CDTT_NguyenMinhDat.Repository;

namespace Web_CDTT_NguyenMinhDat.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin")]
    [Route("admin/home")]
	[Authorize]

	public class ProductAdminController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductAdminController(DataContext context, IWebHostEnvironment webHostEnvironment)
        {
            _dataContext = context;
            _webHostEnvironment = webHostEnvironment;
        }
        [Route("listproduct")]
        public async Task<IActionResult> ListProduct()
        {
            var listProducts = await _dataContext.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .OrderByDescending(p => p.Id)
                .ToListAsync();
            return View(listProducts);
        }

        [Route("listproduct/add")]
        [HttpGet]
        public IActionResult Add()
        {
            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name");
            ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name");
            return View();
        }
        [Route("listproduct/add")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(ProductModel product)
        {
            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", product.CategoryId);
            ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name", product.BrandId);

            if (ModelState.IsValid)
            {
                product.Slug = product.Name.Replace(" ", "-").ToLower(); 
                var existingProduct = await _dataContext.Products
                    .FirstOrDefaultAsync(p => p.Slug == product.Slug);

                if (existingProduct != null)
                {
                    ModelState.AddModelError("", "Sản phẩm đã có trong database");
                    return View(product); 
                }

                if (product.ImageUpload != null)
                {
                    string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                    string imageName = Guid.NewGuid().ToString() + "_" + product.ImageUpload.FileName;

                    string filePath = Path.Combine(uploadsDir, imageName);
                    using (FileStream fs = new FileStream(filePath, FileMode.Create))
                    {
                        await product.ImageUpload.CopyToAsync(fs);
                        fs.Close();
                    }

                    product.Image = imageName;
                }

                _dataContext.Add(product);
                await _dataContext.SaveChangesAsync();
                TempData["Message"] = "Thêm sản phẩm thành công";
                return RedirectToAction("ListProduct");
            }
            else
            {
                TempData["error"] = "Model có một vài thứ đang bị lỗi";
                List<string> errors = new List<string>();
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }
                string errorMessage = string.Join("\n", errors);
                return BadRequest(errorMessage);
            }
            //return View(product);
        }

        
        //edit
        [HttpGet]
        [Route("listproduct/edit")]
        public async Task<IActionResult> Edit(int Id)
        {
            ProductModel product = await _dataContext.Products.FindAsync(Id);
            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", product.CategoryId);
            ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name", product.BrandId);
            return View(product);
        }

        [HttpPost]
        [Route("listproduct/edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int Id, ProductModel product)
        {
            // Tìm sản phẩm dựa vào Id
            var existingProduct = await _dataContext.Products.FindAsync(Id);
            if (existingProduct == null)
            {
                TempData["Message"] = "Sản phẩm không tồn tại.";
                return RedirectToAction("Index");
            }

            // Tiếp tục xử lý cập nhật sản phẩm
            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", product.CategoryId);
            ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name", product.BrandId);

            if (ModelState.IsValid)
            {
                existingProduct.Name = product.Name;
                existingProduct.Description = product.Description;
                existingProduct.Price = product.Price;
                existingProduct.BrandId = product.BrandId;
                existingProduct.CategoryId = product.CategoryId;
                existingProduct.Slug = product.Name.Replace(" ", "-").ToLower();

                // Xử lý cập nhật ảnh
                if (product.ImageUpload != null)
                {
                    string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                    string imageName = Guid.NewGuid().ToString() + "_" + product.ImageUpload.FileName;
                    string filePath = Path.Combine(uploadsDir, imageName);
                    string oldFilePath = Path.Combine(uploadsDir, existingProduct.Image);

                    try
                    {
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "An error occurred while deleting the product image.");
                    }

                    using (FileStream fs = new FileStream(filePath, FileMode.Create))
                    {
                        await product.ImageUpload.CopyToAsync(fs);
                    }

                    existingProduct.Image = imageName;
                }

                _dataContext.Update(existingProduct);
                await _dataContext.SaveChangesAsync();

                TempData["Message"] = "Cập nhật sản phẩm thành công";
                return RedirectToAction("ListProduct");
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
                return BadRequest(errorMessage);
            }
        }




        [Route("listproduct/delete")]
        public async Task<IActionResult> Delete(int Id)
        {
            ProductModel product = await _dataContext.Products.FindAsync(Id);
            if (product == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(product.Image))
            {
                string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                string oldFilePath = Path.Combine(uploadsDir, product.Image);

                try
                {
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while deleting the product image.");
                }
            }

            _dataContext.Products.Remove(product);
            await _dataContext.SaveChangesAsync();
            TempData["Message"] = "Sản phẩm đã được xóa thành công";
            return RedirectToAction("ListProduct");
        }


        //thêm số lượng sản phẩm
        [Route("listproduct/addquantity")]
        [HttpGet]
        public async Task <IActionResult> AddQuantity (int Id)
        {
            var productListQuantity = await _dataContext.ProductsQuantities.Where(p => p.ProductId == Id).ToListAsync();
            ViewBag.ProductByQuantity = productListQuantity;
            ViewBag.Id = Id;
            return View();
        }

        [Route("listproduct/addquantity")]
        [HttpPost]
        public async Task<IActionResult> AddQuantity(ProductsQuantityModel  productsQuantityModel)
        {
            var productquantity = _dataContext.Products.Find(productsQuantityModel.ProductId);

            if (productquantity == null) {

                return NotFound();
            }
            productquantity.Quantity += productsQuantityModel.Quantity;
            productsQuantityModel.Quantity = productsQuantityModel.Quantity;
            productsQuantityModel.ProductId = productsQuantityModel.ProductId;
            productsQuantityModel.DateCreated = DateTime.Now;

            _dataContext.ProductsQuantities.Add(productsQuantityModel);
            _dataContext.SaveChangesAsync ();
            TempData["Message"] = "Thêm số lướng sản phẩm thành công";
            return RedirectToAction("AddQuantity", "ProductAdmin", new { Id=productsQuantityModel.ProductId });
        }

    }

}
