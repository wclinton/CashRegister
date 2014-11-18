using CashRegister.Databases;

namespace CashRegister.Services
{
    public interface IItemDiscountService
    {
        ItemDiscount GetItemDiscountByItemId(int id, string uom);
    }
}
