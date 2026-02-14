using Atfagni.Mobile.ViewModels.Auth;

namespace Atfagni.Mobile.Views.Auth;

public partial class RegisterPage : ContentPage
{
	public RegisterPage(RegisterViewModel ViewModel)
	{
		InitializeComponent();
		BindingContext = ViewModel;
	}
}