using Atfagni.Mobile.Views;
using Atfagni.Mobile.Views.Auth;
using Atfagni.Mobile.Views.Bookings;
using Atfagni.Mobile.Views.Rides;

namespace Atfagni.Mobile
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("RegisterPage", typeof(RegisterPage));
            Routing.RegisterRoute("LoginPage", typeof(LoginPage));
            Routing.RegisterRoute("HomePage", typeof(HomePage));
            Routing.RegisterRoute("PublishRidePage", typeof(PublishRidePage));
            Routing.RegisterRoute("DriverRequestPage", typeof(DriverRequestsPage));
            Routing.RegisterRoute("MyRidesPage", typeof(MyRidesPage));
        }
    }
}
