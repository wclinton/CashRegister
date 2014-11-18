namespace CashRegister.Databases
{
    public class ItemPrice
    {
        public long Id { get; set; }
        public long ItemId { get; set; }
        public decimal  Price { get; set; }
        public string Uom { get; set; }
    }
}
