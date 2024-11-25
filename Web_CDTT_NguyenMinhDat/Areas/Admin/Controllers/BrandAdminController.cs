using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Web_CDTT_NguyenMinhDat.Models;
using Web_CDTT_NguyenMinhDat.Repository;

namespace Web_CDTT_NguyenMinhDat.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin")]
    [Route("admin/home")]

    public class BrandAdminController : Controller
    {

        private readonly DataContext _dataContext;
        public BrandAdminController(DataContext context)
        {
            _dataContext = context;
        }

        [Route("listbrand")]
        public async Task<IActionResult> ListBrand()
        {
            var listBrands = await _dataContext.Brands
                .OrderByDescending(p => p.Id)
                .ToListAsync();
            return View(listBrands);
        }

        //add
        [HttpGet]
        [Route("listbrand/add")]
        public IActionResult Add()
        {
            return View();
        }
        [Route("listbrand/add")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(BrandModel brand)
        {


            if (ModelState.IsValid)
            {
                brand.Slug = brand.Name.Replace(" ", "-").ToLower();
                var existingbrand = await _dataContext.Categories.FirstOrDefaultAsync(p => p.Slug == brand.Slug);

                if (existingbrand != null)
                {
                    ModelState.AddModelError("", "Thương hiện đã có trong database");
                    return View(brand);
                }
                _dataContext.Add(brand);
                await _dataContext.SaveChangesAsync();
                TempData["Message"] = "Thêm thương hiệu thành công";
                return RedirectToAction("ListBrand");
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

        //update
        [Route("listbrand/edit")]
        [HttpGet]
        public async Task<IActionResult> Edit(int Id)
        {
            BrandModel brand = await _dataContext.Brands.FindAsync(Id);
            return View(brand);
        }

        [Route("listbrand/edit")]
        [HttpPost]
        public async Task<IActionResult> Edit(BrandModel brand)
        {
            if (!ModelState.IsValid)
            {
                TempData["Message"] = "Model validation failed";
                return View(brand);
            }

            // Generate slug for the brand
            brand.Slug = Regex.Replace(brand.Name.ToLower(), @"\s+", "-").Replace("--", "-");

            // Check if another brand exists with the same slug
            var existingBrand = await _dataContext.Brands
                .FirstOrDefaultAsync(p => p.Slug == brand.Slug && p.Id != brand.Id);

            if (existingBrand != null)
            {
                ModelState.AddModelError("", "Brand already exists in the database.");
                return View(brand);
            }

            // Retrieve the brand from the database based on Id
            var brandToUpdate = await _dataContext.Brands.FindAsync(brand.Id);
            if (brandToUpdate == null)
            {
                TempData["Message"] = "Brand not found";
                return RedirectToAction("ListBrand");
            }

            // Update necessary fields
            brandToUpdate.Name = brand.Name;
            brandToUpdate.Slug = brand.Slug;

            try
            {
                _dataContext.Entry(brandToUpdate).State = EntityState.Modified;
                await _dataContext.SaveChangesAsync();
                TempData["Message"] = "Brand updated successfully!";
                return RedirectToAction("ListBrand");
            }
            catch (Exception ex)
            {
                TempData["Message"] = "An error occurred while updating the brand.";
                return View(brand);
            }
        }



        //delete
        [Route("listbrand/delete")]
        public async Task<IActionResult> Delete(int Id)
        {
            BrandModel brands = await _dataContext.Brands.FindAsync(Id);

            _dataContext.Brands.Remove(brands);
            await _dataContext.SaveChangesAsync();
            TempData["Message"] = "Xóa thương hiệu thành công !";
            return RedirectToAction("ListBrand");
        }
    }
}
