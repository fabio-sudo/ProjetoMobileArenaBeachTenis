using System.Threading.Tasks;
using ArenaPOS.Models;

namespace ArenaPOS.Interfaces
{
    public interface ICashRegisterService
    {
        Task<CashRegister> GetCurrentCashRegisterAsync();
        Task<bool> OpenCashRegisterAsync(decimal startingBalance);
        Task<bool> AddExpenseAsync(decimal amount, string description);
        Task<bool> AddAdjustmentAsync(decimal amount, string description);
        Task<bool> CloseCashRegisterAsync(decimal closingBalance);
    }
}
