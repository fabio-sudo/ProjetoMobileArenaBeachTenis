using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ArenaPOS.Interfaces;
using ArenaPOS.Models;
using ArenaPOS.Storage;

namespace ArenaPOS.Services
{
    public class SalesService : ISalesService
    {
        private readonly HttpClient _httpClient;
        private readonly LocalStore _localStore;

        public SalesService(HttpClient httpClient, LocalStore localStore)
        {
            _httpClient = httpClient;
            _localStore = localStore;
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            await _localStore.Init();
            
            try
            {
                // Try fetch from API
                var products = await _httpClient.GetFromJsonAsync<List<Product>>("api/products");
                
                if (products != null)
                {
                    await _localStore.Connection.DeleteAllAsync<Product>();
                    await _localStore.Connection.InsertAllAsync(products);
                    return products;
                }
            }
            catch
            {
                // Fallback to local
            }

            return await _localStore.Connection.Table<Product>().ToListAsync();
        }

        public async Task<bool> SubmitSaleAsync(Sale sale)
        {
            await _localStore.Init();
            
            try
            {
                // Try send to API immediately
                var response = await _httpClient.PostAsJsonAsync("api/sales", sale);
                if (response.IsSuccessStatusCode)
                {
                    sale.IsSynced = true;
                    // Mock ApiId return
                    sale.ApiId = new Random().Next(1000, 9999);
                }
            }
            catch
            {
                sale.IsSynced = false;
            }

            // Save locally (synced or pending)
            await _localStore.Connection.InsertAsync(sale);
            foreach (var item in sale.Items)
            {
                item.SaleLocalId = sale.LocalId;
                await _localStore.Connection.InsertAsync(item);
            }

            return true;
        }

        public async Task SyncPendingSalesAsync()
        {
            await _localStore.Init();
            var pendingSales = await _localStore.Connection.Table<Sale>().Where(s => !s.IsSynced).ToListAsync();

            foreach (var sale in pendingSales)
            {
                try
                {
                    // Fetch items manually for the sale since SQLiteNetExtensions might not be queried here
                    sale.Items = await _localStore.Connection.Table<SaleItem>().Where(i => i.SaleLocalId == sale.LocalId).ToListAsync();

                    var response = await _httpClient.PostAsJsonAsync("api/sales", sale);
                    if (response.IsSuccessStatusCode)
                    {
                        sale.IsSynced = true;
                        await _localStore.Connection.UpdateAsync(sale);
                    }
                }
                catch
                {
                    // Continue to next, remain pending
                }
            }
        }
    }
}