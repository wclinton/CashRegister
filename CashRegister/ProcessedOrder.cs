using System.Collections.Generic;
using System.Linq;

namespace CashRegister
{
    public class ProcessedOrder
    {

        public ProcessedOrder(Order order, IEnumerable<ProcessedOrderItem> processedOrderItems,decimal total)
        {           
            ProcessedOrderItems = processedOrderItems.ToList();
            Total = total;
            CouponCodes = order.CouponCodes;
        }
     
        public decimal Total { get; set; }

        public IList<ProcessedOrderItem> ProcessedOrderItems;
        public  IEnumerable<string> CouponCodes;

    }
}
