using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using ArenaPOS.Interfaces;
using ArenaPOS.Models;
using System;

namespace ArenaPOS.ViewModels
{
    public partial class CashRegisterViewModel : ObservableObject
    {
        private readonly ICashRegisterService _cashService;

        [ObservableProperty]
        private CashRegister currentCash;

        [ObservableProperty]
        private decimal expenseAmount;

        [ObservableProperty]
        private string expenseDescription;

        [ObservableProperty]
        private bool isCashOpen;

        public CashRegisterViewModel(ICashRegisterService cashService)
        {
            _cashService = cashService;
            LoadCashAsync().ConfigureAwait(false);
        }

        [RelayCommand]
        private async Task LoadCashAsync()
        {
            CurrentCash = await _cashService.GetCurrentCashRegisterAsync();
            IsCashOpen = CurrentCash != null && CurrentCash.Status == "Open";
        }

        [RelayCommand]
        private async Task OpenCashAsync()
        {
            string startBalance = await Shell.Current.DisplayPromptAsync("Abrir Caixa", "Saldo Inicial (R$):", keyboard: Keyboard.Numeric);
            if (decimal.TryParse(startBalance, out decimal balance))
            {
                if (await _cashService.OpenCashRegisterAsync(balance))
                {
                    await LoadCashAsync();
                }
            }
        }

        [RelayCommand]
        private async Task CloseCashAsync()
        {
            if (CurrentCash == null) return;

            string endBalance = await Shell.Current.DisplayPromptAsync("Fechar Caixa", "Saldo Final (R$):", keyboard: Keyboard.Numeric);
            if (decimal.TryParse(endBalance, out decimal balance))
            {
                if (await _cashService.CloseCashRegisterAsync(balance))
                {
                    await LoadCashAsync();
                }
            }
        }

        [RelayCommand]
        private async Task AddExpenseAsync()
        {
            if (ExpenseAmount <= 0 || string.IsNullOrWhiteSpace(ExpenseDescription)) return;

            if (await _cashService.AddExpenseAsync(ExpenseAmount, ExpenseDescription))
            {
                ExpenseAmount = 0;
                ExpenseDescription = string.Empty;
                await Shell.Current.DisplayAlertAsync("Lançamento", "Despesa registrada com sucesso.", "OK");
            }
        }
    }
}
