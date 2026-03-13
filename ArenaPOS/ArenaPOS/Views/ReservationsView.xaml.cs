using ArenaPOS.ViewModels;

namespace ArenaPOS.Views
{
    public partial class ReservationsView : ContentPage
    {
        public ReservationsView(ReservationsViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
