using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CashRegister.Databases;

namespace CashRegister.Services
{
    public interface ICouponService
    {
        Coupon GetCoupon(string code);
    }
}
