namespace CashRegister.Databases
{
    public class Coupon
    {
        public long Id { get; set; }

        public string Code { get; set; }

        public decimal MinValue { get; set; }

        public decimal Discount { get; set; }
    }
}
