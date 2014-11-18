using System.Data.Entity;
using CashRegister.Databases;

namespace CashRegister.Databases
{
    public interface IContext
    {
         DbSet<Item> Items { get; set; }

         DbSet<ItemPrice> ItemPrices { get; set; }

         DbSet<Coupon> Coupons { get; set; }

         DbSet<ItemDiscount> ItemDiscounts { get; set; }
    }
}
