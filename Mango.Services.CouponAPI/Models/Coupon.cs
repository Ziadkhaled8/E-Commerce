using System.ComponentModel.DataAnnotations;

namespace Mango.Services.ShoppingCartAPI.Models
{
    public class Coupon
    {
        [Key]
        public int CouponId { get; set; }
        public string CouponCode { get; set; }
        public double DiscountAmount {  get; set; }
        public int MinAmount { get; set; }
    }
}
