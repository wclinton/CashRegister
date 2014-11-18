using System.Linq;
using CashRegister.Databases;

namespace CashRegister.Services
{
    public class ItemService:IItemService
    {
          private readonly IContext db;
          public ItemService(IContext db)
        {
            this.db = db;
        }

        public Item Get(int id)
        {
            return db.Items.SingleOrDefault(x => x.Id == id);
        }
    }
}
