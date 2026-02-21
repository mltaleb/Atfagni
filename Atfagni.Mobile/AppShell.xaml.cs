using Atfagni.Mobile.Views;
using Atfagni.Mobile.Views.Auth;
using Atfagni.Mobile.Views.Bookings;
using Atfagni.Mobile.Views.Rides;
using Atfagni.Mobile.ViewModels;

namespace Atfagni.Mobile
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            // On attache le ViewModel pour pouvoir utiliser le Binding dans le XAML
            BindingContext = new AppShellViewModel();
            Routing.RegisterRoute("RegisterPage", typeof(RegisterPage));
            Routing.RegisterRoute("LoginPage", typeof(LoginPage));
            Routing.RegisterRoute("HomePage", typeof(HomePage));
            Routing.RegisterRoute("PublishRidePage", typeof(PublishRidePage));
            Routing.RegisterRoute("DriverRequestPage", typeof(DriverRequestsPage));
            Routing.RegisterRoute("MyRidesPage", typeof(MyRidesPage));
            // Ajoutez la route pour la navigation
            Routing.RegisterRoute("BookingHistoryPage", typeof(Views.Rides.BookingHistoryPage));
            Routing.RegisterRoute("RideDetailPage", typeof(RideDetailPage));
            Routing.RegisterRoute("MyBookingsPage", typeof(MyBookingsPage));
            
        }
    }
}
