using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ArenaPOS.Interfaces;
using ArenaPOS.Models;
using ArenaPOS.Storage;
using Tab = ArenaPOS.Models.Tab;

namespace ArenaPOS.Services
{
    public class TabsService : ITabsService
    {
        private readonly HttpClient _httpClient;
        private readonly LocalStore _localStore;

        public TabsService(HttpClient httpClient, LocalStore localStore)
        {
            _httpClient = httpClient;
            _localStore = localStore;
        }

        public async Task<List<Tab>> GetOpenTabsAsync()
        {
            await _localStore.Init();
            
            try
            {
                var tabs = await _httpClient.GetFromJsonAsync<List<Tab>>("api/tabs");
                if (tabs != null)
                {
                    // Update local cache
                    foreach(var tab in tabs)
                    {
                        var localTab = await _localStore.Connection.Table<Tab>().Where(t => t.ApiId == tab.ApiId).FirstOrDefaultAsync();
                        if (localTab == null)
                        {
                            tab.IsSynced = true;
                            await _localStore.Connection.InsertAsync(tab);
                        }
                    }
                }
            }
            catch { }

            return await _localStore.Connection.Table<Tab>().Where(t => t.Status == "Open").ToListAsync();
        }

        public async Task<Tab> OpenTabAsync(string customerName)
        {
            await _localStore.Init();
            var newTab = new Tab { CustomerName = customerName, Status = "Open", TotalAmount = 0 };

            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/tabs", new { CustomerName = customerName });
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<Tab>();
                    newTab.ApiId = result?.ApiId ?? 0;
                    newTab.IsSynced = true;
                }
            }
            catch { }

            await _localStore.Connection.InsertAsync(newTab);
            return newTab;
        }

        public async Task<bool> AddItemToTabAsync(int tabLocalId, TabItem item)
        {
            await _localStore.Init();
            var tab = await _localStore.Connection.FindAsync<Tab>(tabLocalId);
            if (tab == null) return false;

            item.TabLocalId = tabLocalId;
            
            try
            {
                if (tab.IsSynced)
                {
                    var response = await _httpClient.PostAsJsonAsync($"api/tabs/{tab.ApiId}/items", item);
                    if (response.IsSuccessStatusCode)
                    {
                        item.IsSynced = true;
                    }
                }
            }
            catch { }

            await _localStore.Connection.InsertAsync(item);
            tab.TotalAmount += (item.Price * item.Quantity);
            await _localStore.Connection.UpdateAsync(tab);

            return true;
        }

        public async Task<bool> RemoveItemFromTabAsync(int tabLocalId, int itemLocalId)
        {
            await _localStore.Init();
            var item = await _localStore.Connection.FindAsync<TabItem>(itemLocalId);
            if (item == null) return false;

            var tab = await _localStore.Connection.FindAsync<Tab>(tabLocalId);
            
            try
            {
                if (tab != null && tab.IsSynced && item.IsSynced)
                {
                    await _httpClient.DeleteAsync($"api/tabs/{tab.ApiId}/items/{item.ApiId}");
                }
            }
            catch { }

            if (tab != null)
            {
                tab.TotalAmount -= (item.Price * item.Quantity);
                await _localStore.Connection.UpdateAsync(tab);
            }

            await _localStore.Connection.DeleteAsync(item);
            return true;
        }

        public async Task<bool> CloseTabAsync(int tabLocalId, string paymentMethod)
        {
            await _localStore.Init();
            var tab = await _localStore.Connection.FindAsync<Tab>(tabLocalId);
            if (tab == null) return false;

            tab.Status = "Closed";

            try
            {
                var response = await _httpClient.PostAsync($"api/tabs/{tab.ApiId}/close", null);
                if (response.IsSuccessStatusCode)
                {
                    tab.IsSynced = true;
                }
                else
                {
                    tab.IsSynced = false;
                }
            }
            catch
            {
                tab.IsSynced = false;
            }

            await _localStore.Connection.UpdateAsync(tab);
            return true;
        }
    }
}
