using Atfagni.Mobile.ViewModels.Bookings;

namespace Atfagni.Mobile.Views.Bookings;

public partial class DriverRequestsPage : ContentPage
{
	public DriverRequestsPage(DriverRequestsViewModel ViewModel)
	{
		InitializeComponent();
		BindingContext = ViewModel;
	}
}