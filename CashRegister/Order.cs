using System.Collections.Generic;

namespace CashRegister
{
    public class Order
    {
        public IList<OrderItem> OrderItems;
        public ICollection<string> CouponCodes;
    }
}
