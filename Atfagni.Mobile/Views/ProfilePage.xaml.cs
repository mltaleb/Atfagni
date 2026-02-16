namespace Atfagni.Mobile.Views;

public partial class ProfilePage : ContentPage
{
    public ProfilePage(ViewModels.ProfileViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}