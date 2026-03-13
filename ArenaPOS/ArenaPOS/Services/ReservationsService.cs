using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ArenaPOS.Interfaces;
using ArenaPOS.Models;
using ArenaPOS.Storage;

namespace ArenaPOS.Services
{
    public class ReservationsService : IReservationsService
    {
        private readonly HttpClient _httpClient;
        private readonly LocalStore _localStore;

        public ReservationsService(HttpClient httpClient, LocalStore localStore)
        {
            _httpClient = httpClient;
            _localStore = localStore;
        }

        public async Task<List<Reservation>> GetTodaysReservationsAsync()
        {
            await _localStore.Init();
            try
            {
                var reservations = await _httpClient.GetFromJsonAsync<List<Reservation>>("api/reservations/today");
                if (reservations != null)
                {
                    await _localStore.Connection.DeleteAllAsync<Reservation>();
                    await _localStore.Connection.InsertAllAsync(reservations);
                    return reservations;
                }
            }
            catch { }

            return await _localStore.Connection.Table<Reservation>().ToListAsync();
        }

        public async Task<bool> CheckoutReservationAsync(int apiId, string paymentMethod)
        {
            await _localStore.Init();
            
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"api/reservations/{apiId}/checkout", new { PaymentMethod = paymentMethod });
                if (response.IsSuccessStatusCode)
                {
                    var reservation = await _localStore.Connection.Table<Reservation>().Where(r => r.ApiId == apiId).FirstOrDefaultAsync();
                    if (reservation != null)
                    {
                        reservation.Status = "Paid";
                        await _localStore.Connection.UpdateAsync(reservation);
                    }
                    return true;
                }
            }
            catch { }

            return false;
        }
    }
}
