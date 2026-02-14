using Atfagni.Mobile.ViewModels.Rides;

namespace Atfagni.Mobile.Views.Rides;

public partial class PublishRidePage : ContentPage
{
	public PublishRidePage(PublishRideViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}