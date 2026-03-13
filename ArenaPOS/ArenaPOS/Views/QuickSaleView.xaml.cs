using ArenaPOS.ViewModels;

namespace ArenaPOS.Views;

public partial class QuickSaleView : ContentPage
{
    public QuickSaleView(QuickSaleViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}