using Atfagni.Mobile.Services;
using Atfagni.Mobile.ViewModels;
using Atfagni.Mobile.ViewModels.Auth;
using Atfagni.Mobile.ViewModels.Bookings;
using Atfagni.Mobile.ViewModels.Rides;
using Atfagni.Mobile.Views;
using Atfagni.Mobile.Views.Auth;
using Atfagni.Mobile.Views.Bookings;
using Atfagni.Mobile.Views.Rides;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

namespace Atfagni.Mobile
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            builder.Services.AddSingleton<ApiService>();
            builder.Services.AddSingleton<DatabaseService>();
            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddSingleton<MainViewModel>();
            builder.Services.AddTransient<RegisterViewModel>();
            builder.Services.AddTransient<RegisterPage>();
            // Ajoutez ces lignes avec les autres services
            builder.Services.AddTransient<HomeViewModel>();
            builder.Services.AddTransient<HomePage>();
            // Ajoutez ces lignes avec les autres services
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<DriverRequestsViewModel>();
            builder.Services.AddTransient<DriverRequestsPage>();
            builder.Services.AddTransient<PublishRideViewModel>();
            builder.Services.AddTransient<PublishRidePage>();
            builder.Services.AddTransient<MyRidesViewModel>();
            builder.Services.AddTransient<MyRidesPage>();// Ajoutez ces lignes
            builder.Services.AddTransient<ViewModels.Rides.BookingHistoryViewModel>();
            builder.Services.AddTransient<Views.Rides.BookingHistoryPage>();
            builder.Services.AddTransient<RideDetailViewModel>();
            builder.Services.AddTransient<RideDetailPage>();
            builder.Services.AddTransient<MyBookingsViewModel>();
            builder.Services.AddTransient<MyBookingsPage>();

            // Profil
            builder.Services.AddTransient<ProfileViewModel>();
            builder.Services.AddTransient<ProfilePage>();
            return builder.Build();
        }
    }
}
