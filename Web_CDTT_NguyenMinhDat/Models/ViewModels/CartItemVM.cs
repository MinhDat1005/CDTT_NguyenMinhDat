namespace Web_CDTT_NguyenMinhDat.Models.ViewModels
{
    public class CartItemVM
    {
        public List<CartItemModel> CartItems { get; set; }
        public decimal GrandTotal { get; set; }

        public decimal ShippingCOD { get;set; }
        public string CouponCOD { get; set; }
    }
}
