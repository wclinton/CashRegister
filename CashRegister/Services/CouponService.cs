using System.Linq;
using CashRegister.Databases;

namespace CashRegister.Services
{
   public  class CouponService:ICouponService
   {
       private readonly IContext db;
       public CouponService(IContext db)
       {
           this.db = db;
       }

       public Coupon GetCoupon(string code)
       {
           return code == null ? null : db.Coupons.SingleOrDefault(x => x.Code == code);
       }
    }
}
