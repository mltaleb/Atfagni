using Atfagni.Mobile.ViewModels.Auth;

namespace Atfagni.Mobile.Views.Auth;

public partial class LoginPage : ContentPage
{
	public LoginPage(LoginViewModel ViewModel)
	{
		InitializeComponent();
		BindingContext = ViewModel;
	}
}