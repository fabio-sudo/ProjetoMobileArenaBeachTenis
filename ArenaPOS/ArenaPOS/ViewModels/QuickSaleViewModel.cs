using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ArenaPOS.Models;
using ArenaPOS.Interfaces;
using System.Collections.ObjectModel;

namespace ArenaPOS.ViewModels;

public partial class QuickSaleViewModel : ObservableObject
{
    private readonly ISalesService _salesService;

    public ObservableCollection<Product> Products { get; set; } = new();

    public ObservableCollection<SaleItem> Cart { get; set; } = new();

    [ObservableProperty]
    decimal total;

    public QuickSaleViewModel(ISalesService salesService)
    {
        _salesService = salesService;
        LoadProductsCommand.Execute(null);
    }

    [RelayCommand]
    async Task LoadProducts()
    {
        var products = await _salesService.GetProductsAsync();

        Products.Clear();

        foreach (var product in products)
            Products.Add(product);
    }

    [RelayCommand]
    void AddToCart(Product product)
    {
        var existing = Cart.FirstOrDefault(i => i.ProductId == product.Id);
        if (existing != null)
        {
            existing.Quantity++;
            // Notify change if using ObservableCollection properly, but rebuilding list or relying on INotifyPropetyChanged on SaleItem works if SaleItem is ObservableObject.
            // Since it's quick sale, let's just replace it to force update or we assume SaleItem has PropertyChanged.
            Cart.Remove(existing);
            Cart.Add(existing);
        }
        else
        {
            Cart.Add(new SaleItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                Price = product.Price,
                Quantity = 1
            });
        }

        CalculateTotal();
    }

    [RelayCommand]
    void ClearCart()
    {
        Cart.Clear();
        CalculateTotal();
    }

    [RelayCommand]
    async Task CheckoutAsync()
    {
        if (Cart.Count == 0) return;

        string paymentMethod = await Shell.Current.DisplayActionSheetAsync("Forma de Pagamento", "Cancelar", null, "Dinheiro", "Cartão de Crédito", "Cartão de Débito", "Pix");

        if (paymentMethod != "Cancelar" && !string.IsNullOrEmpty(paymentMethod))
        {
            var sale = new Sale
            {
                Total = Total,
                PaymentMethod = paymentMethod,
                Items = Cart.ToList()
            };

            var success = await _salesService.SubmitSaleAsync(sale);
            
            if (success)
            {
                await Shell.Current.DisplayAlertAsync("Sucesso", "Venda finalizada com sucesso!", "OK");
                ClearCart();
            }
            else
            {
                await Shell.Current.DisplayAlertAsync("Erro", "Houve um erro ao registrar a venda.", "OK");
            }
        }
    }

    void CalculateTotal()
    {
        Total = Cart.Sum(x => x.Price * x.Quantity);
    }
}