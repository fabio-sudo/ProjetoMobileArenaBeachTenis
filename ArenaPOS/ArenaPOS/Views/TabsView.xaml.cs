using ArenaPOS.ViewModels;

namespace ArenaPOS.Views
{
    public partial class TabsView : ContentPage
    {
        public TabsView(TabsViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
