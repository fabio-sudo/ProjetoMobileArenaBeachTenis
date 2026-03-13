using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ArenaPOS.Interfaces;

namespace ArenaPOS.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly IAuthService _authService;

        [ObservableProperty]
        private string email = "admin";

        [ObservableProperty]
        private string password = "admin123";

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        public LoginViewModel(IAuthService authService)
        {
            _authService = authService;
        }

        [RelayCommand]
        private async Task LoginAsync()
        {
            if (string.IsNullOrWhiteSpace(this.Email) || string.IsNullOrWhiteSpace(this.Password))
            {
                this.ErrorMessage = "Preencha todos os campos.";
                return;
            }

            this.IsBusy = true;
            this.ErrorMessage = string.Empty;

            var token = await _authService.LoginAsync(this.Email, this.Password);

            this.IsBusy = false;

            if (!string.IsNullOrEmpty(token))
            {
                await Shell.Current.GoToAsync("//Main");
            }
            else
            {
                this.ErrorMessage = "Credenciais inválidas ou erro na API.";
            }
        }
    }
}
