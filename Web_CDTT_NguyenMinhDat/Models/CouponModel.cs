namespace Web_CDTT_NguyenMinhDat.Models
{
    public class CouponModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedStart { get; set; }
        public DateTime CreatedExpired { get; set; }
        public int Status { get; set; }
        public decimal PriceCoupon { get; set; }
        public decimal DiscountPrice { get; set; }
    }
}
