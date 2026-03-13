using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ArenaPOS.Interfaces;
using ArenaPOS.Models;
using ArenaPOS.Storage;

namespace ArenaPOS.Services
{
    public class CashRegisterService : ICashRegisterService
    {
        private readonly HttpClient _httpClient;
        private readonly LocalStore _localStore;

        public CashRegisterService(HttpClient httpClient, LocalStore localStore)
        {
            _httpClient = httpClient;
            _localStore = localStore;
        }

        public async Task<CashRegister> GetCurrentCashRegisterAsync()
        {
            await _localStore.Init();
            try
            {
                var cash = await _httpClient.GetFromJsonAsync<CashRegister>("api/cash/current");
                if (cash != null)
                {
                    cash.IsSynced = true;
                    // Mock behavior to overwrite local for now
                    await _localStore.Connection.DeleteAllAsync<CashRegister>();
                    await _localStore.Connection.InsertAsync(cash);
                    return cash;
                }
            }
            catch { }

            return await _localStore.Connection.Table<CashRegister>().Where(c => c.Status == "Open").FirstOrDefaultAsync();
        }

        public async Task<bool> OpenCashRegisterAsync(decimal startingBalance)
        {
            await _localStore.Init();
            var newCash = new CashRegister { DateOpened = DateTime.UtcNow, StartingBalance = startingBalance, Status = "Open" };

            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/cash/open", new { StartingBalance = startingBalance });
                if (response.IsSuccessStatusCode)
                {
                    newCash.IsSynced = true;
                }
            }
            catch { }

            await _localStore.Connection.InsertAsync(newCash);
            return true;
        }

        public async Task<bool> AddExpenseAsync(decimal amount, string description)
        {
            await _localStore.Init();
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/cash/expense", new { Amount = amount, Description = description });
                if (response.IsSuccessStatusCode)
                {
                    // Register local payment transaction
                    var payment = new Payment { Amount = -amount, Date = DateTime.UtcNow, Source = "Expense", Method = "Cash", IsSynced = true };
                    await _localStore.Connection.InsertAsync(payment);
                    return true;
                }
            }
            catch { }

            // Offline
            var offlinePayment = new Payment { Amount = -amount, Date = DateTime.UtcNow, Source = "Expense", Method = "Cash", IsSynced = false };
            await _localStore.Connection.InsertAsync(offlinePayment);
            return true;
        }

        public async Task<bool> AddAdjustmentAsync(decimal amount, string description)
        {
            await _localStore.Init();
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/cash/adjustment", new { Amount = amount, Description = description });
                if (response.IsSuccessStatusCode)
                {
                    var payment = new Payment { Amount = amount, Date = DateTime.UtcNow, Source = "Adjustment", Method = "Cash", IsSynced = true };
                    await _localStore.Connection.InsertAsync(payment);
                    return true;
                }
            }
            catch { }

            var offlinePayment = new Payment { Amount = amount, Date = DateTime.UtcNow, Source = "Adjustment", Method = "Cash", IsSynced = false };
            await _localStore.Connection.InsertAsync(offlinePayment);
            return true;
        }

        public async Task<bool> CloseCashRegisterAsync(decimal closingBalance)
        {
            await _localStore.Init();
            var current = await GetCurrentCashRegisterAsync();
            if (current == null) return false;

            current.ClosingBalance = closingBalance;
            current.DateClosed = DateTime.UtcNow;
            current.Status = "Closed";

            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/cash/close", new { ClosingBalance = closingBalance });
                if (response.IsSuccessStatusCode)
                {
                    current.IsSynced = true;
                }
            }
            catch { }

            await _localStore.Connection.UpdateAsync(current);
            return true;
        }
    }
}
