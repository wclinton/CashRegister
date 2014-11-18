using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CashRegister.Databases;

namespace CashRegister.Services
{
    class ItemDiscountService:IItemDiscountService
    {

        private readonly IContext db;
        public ItemDiscountService(IContext db)
        {
            this.db = db;
        }

        public ItemDiscount GetItemDiscountByItemId(int id, string uom)
        {
            return db.ItemDiscounts.SingleOrDefault(x => x.ItemId == id && x.Uom == uom);
        }
    }
}
