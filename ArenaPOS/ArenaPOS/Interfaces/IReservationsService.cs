using System.Collections.Generic;
using System.Threading.Tasks;
using ArenaPOS.Models;

namespace ArenaPOS.Interfaces
{
    public interface IReservationsService
    {
        Task<List<Reservation>> GetTodaysReservationsAsync();
        Task<bool> CheckoutReservationAsync(int apiId, string paymentMethod);
    }
}
