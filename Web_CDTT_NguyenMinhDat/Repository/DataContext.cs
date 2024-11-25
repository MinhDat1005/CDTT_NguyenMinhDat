using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Web_CDTT_NguyenMinhDat.Models;

namespace Web_CDTT_NguyenMinhDat.Repository
{
	public class DataContext:IdentityDbContext<AppUserModel>
	{
		public DataContext(DbContextOptions<DataContext> options):base (options) {
		}
		public DbSet<BrandModel> Brands { get; set; }
		public DbSet<CategoryModel> Categories { get; set; }
		public DbSet<ProductModel> Products { get; set; }
		public DbSet<OrderDetail> OrderDetails { get; set; }
		public DbSet<OrderModel> OrderModels { get; set; }
		public DbSet<RatingModel> Ratings { get; set; }
		public DbSet<WishListModel> WishLists { get; set; }
		public DbSet<CompareModel> Compares { get; set; }

		public DbSet<ProductsQuantityModel> ProductsQuantities { get; set; }
		public DbSet<ContactModel> Contacts { get; set; }
		public DbSet<ShippingModel> Shippings { get; set; }
		public DbSet<CouponModel> Coupons { get; set; }
		public DbSet<StatisticalModel> StatisticalModels { get; set; }
	}
}
