using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ArenaPOS.Interfaces;
using ArenaPOS.Models;
using Reservation = ArenaPOS.Models.Reservation;

namespace ArenaPOS.ViewModels
{
    public partial class ReservationsViewModel : ObservableObject
    {
        private readonly IReservationsService _reservationsService;

        public ObservableCollection<Reservation> TodaysReservations { get; set; } = new();

        public ReservationsViewModel(IReservationsService reservationsService)
        {
            _reservationsService = reservationsService;
            LoadReservationsCommand.Execute(null);
        }

        [RelayCommand]
        private async Task LoadReservationsAsync()
        {
            var reservations = await _reservationsService.GetTodaysReservationsAsync();
            TodaysReservations.Clear();
            foreach(var r in reservations)
            {
                TodaysReservations.Add(r);
            }
        }

        [RelayCommand]
        private async Task CheckoutReservationAsync(Reservation res)
        {
            if (res == null || res.Status == "Paid") return;

            string method = await Shell.Current.DisplayActionSheetAsync("Forma de Pagamento", "Cancelar", null, "Cartão Crédito", "Cartão Débito", "Pix", "Dinheiro");
            
            if (method != "Cancelar" && !string.IsNullOrEmpty(method))
            {
                bool success = await _reservationsService.CheckoutReservationAsync(res.ApiId, method);
                if (success)
                {
                    await Shell.Current.DisplayAlertAsync("Sucesso", "Pagamento de reserva registrado.", "OK");
                    await LoadReservationsAsync();
                }
                else
                {
                    await Shell.Current.DisplayAlertAsync("Erro", "Falha ao registrar pagamento.", "OK");
                }
            }
        }
    }
}
