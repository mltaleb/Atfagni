using Atfagni.Mobile.ViewModels;

namespace Atfagni.Mobile.Views;

public partial class HomePage : ContentPage
{
	public HomePage(HomeViewModel ViewModel)
	{
		InitializeComponent();
		BindingContext = ViewModel;
	}
}