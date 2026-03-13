using ArenaPOS.ViewModels;

namespace ArenaPOS.Views
{
    public partial class CashRegisterView : ContentPage
    {
        public CashRegisterView(CashRegisterViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
