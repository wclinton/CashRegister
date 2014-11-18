namespace CashRegister.Databases
{
    public class ItemDiscount
    {
        public long Id { get; set; }
        public long ItemId { get; set; }

        public string Uom { get; set; }
        public decimal MinQuantity { get; set; }
        public decimal DiscountQuantity { get; set; }        
    }
}
