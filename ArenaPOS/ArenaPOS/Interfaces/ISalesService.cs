using System.Collections.Generic;
using System.Threading.Tasks;
using ArenaPOS.Models;

namespace ArenaPOS.Interfaces
{
    public interface ISalesService
    {
        Task<List<Product>> GetProductsAsync();
        Task<bool> SubmitSaleAsync(Sale sale);
        Task SyncPendingSalesAsync();
    }
}