using ArenaPOS.Helpers;
using ArenaPOS.Interfaces;
using ArenaPOS.Services;
using ArenaPOS.ViewModels;
using ArenaPOS.Views;
using Microsoft.Extensions.Logging;
using CommunityToolkit.Mvvm;
using ArenaPOS.Storage;


namespace ArenaPOS
{
    public static class MauiProgram
    {

        public static MauiApp CreateMauiApp()
        {

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });


            // Register HttpClient with Base Address
            builder.Services.AddHttpClient("Api", client =>
            {
                client.BaseAddress = new Uri("http://localhost:5000/"); // Default local backend port
            });

            // Make default HttpClient available
            builder.Services.AddTransient(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("Api"));

            // Local Store setup
            builder.Services.AddSingleton<ArenaPOS.Storage.LocalStore>();

            // Services
            builder.Services.AddSingleton<IAuthService, AuthService>();
            builder.Services.AddSingleton<ISalesService, SalesService>();
            builder.Services.AddSingleton<ITabsService, TabsService>();
            builder.Services.AddSingleton<ICashRegisterService, CashRegisterService>();
            builder.Services.AddSingleton<IReservationsService, ReservationsService>();

            // Authentication & Views
            builder.Services.AddSingleton<LoginViewModel>();
            builder.Services.AddSingleton<LoginView>();

            // Main Modules
            builder.Services.AddSingleton<QuickSaleViewModel>();
            builder.Services.AddSingleton<QuickSaleView>();
            
            builder.Services.AddSingleton<TabsViewModel>();
            builder.Services.AddSingleton<TabsView>();
            
            builder.Services.AddSingleton<CashRegisterViewModel>();
            builder.Services.AddSingleton<CashRegisterView>();

            builder.Services.AddSingleton<ReservationsViewModel>();
            builder.Services.AddSingleton<ReservationsView>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
