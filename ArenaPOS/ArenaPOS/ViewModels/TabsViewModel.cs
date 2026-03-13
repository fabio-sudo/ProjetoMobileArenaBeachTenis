using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ArenaPOS.Interfaces;
using ArenaPOS.Models;
using Tab = ArenaPOS.Models.Tab;

namespace ArenaPOS.ViewModels
{
    public partial class TabsViewModel : ObservableObject
    {
        private readonly ITabsService _tabsService;

        public ObservableCollection<Tab> OpenTabs { get; set; } = new();

        public TabsViewModel(ITabsService tabsService)
        {
            _tabsService = tabsService;
            LoadTabsCommand.Execute(null);
        }

        [RelayCommand]
        private async Task LoadTabsAsync()
        {
            var tabs = await _tabsService.GetOpenTabsAsync();
            OpenTabs.Clear();
            foreach (var tab in tabs)
                OpenTabs.Add(tab);
        }

        [RelayCommand]
        private async Task OpenNewTabAsync()
        {
            string customerName = await Shell.Current.DisplayPromptAsync("Nova Comanda", "Nome do Cliente:");
            if (!string.IsNullOrWhiteSpace(customerName))
            {
                var tab = await _tabsService.OpenTabAsync(customerName);
                if (tab != null)
                {
                    OpenTabs.Add(tab);
                }
            }
        }
    }
}
