using System.Data.Entity;

namespace CashRegister.Databases
{
   public  class CashRegisterContext:   DbContext, IContext
    {
        public DbSet<Item> Items { get; set; }

        public DbSet<ItemPrice> ItemPrices { get; set; }

        public DbSet<Coupon> Coupons { get; set; }

        public DbSet<ItemDiscount> ItemDiscounts { get; set; }
    }
}
