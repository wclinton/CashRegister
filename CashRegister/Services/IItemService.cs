using CashRegister.Databases;

namespace CashRegister.Services
{
    public interface IItemService
    {
        Item Get(int id);
    }
}
