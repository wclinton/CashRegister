using CashRegister.Databases;

namespace CashRegister
{
    public class ProcessedOrderItem
    {

        public Item Item { get; set; }
        public decimal Quantity { get; set; }

        public string Uom { get; set; }

        public bool HasDiscount { get; set; }

        public decimal Total { get; set; }

        public decimal Price { get; set; }
    }
}
