using ArenaPOS.ViewModels;

namespace ArenaPOS.Views
{
    public partial class LoginView : ContentPage
    {
        public LoginView(LoginViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
