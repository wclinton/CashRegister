using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CashRegister.Databases;

namespace CashRegister.Services
{
    public class ItemPriceService : IItemPriceService
    {
        private readonly IContext db;

        public ItemPriceService(IContext db)
        {
            this.db = db;
        }

        public ItemPrice GetItemPriceByItemId(int id, string uom)
        {
            return db.ItemPrices.SingleOrDefault(x => x.ItemId == id && x.Uom == uom);
        }
    }
}
