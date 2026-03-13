using System.Collections.Generic;
using System.Threading.Tasks;
using ArenaPOS.Models;
using Tab = ArenaPOS.Models.Tab;

namespace ArenaPOS.Interfaces
{
    public interface ITabsService
    {
        Task<List<Tab>> GetOpenTabsAsync();
        Task<Tab> OpenTabAsync(string customerName);
        Task<bool> AddItemToTabAsync(int tabLocalId, TabItem item);
        Task<bool> RemoveItemFromTabAsync(int tabLocalId, int itemLocalId);
        Task<bool> CloseTabAsync(int tabLocalId, string paymentMethod);
    }
}
