using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Atfagni.Mobile.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [RelayCommand]
    private async Task GoToRegister()
    {
        // Navigation vers la page d'inscription via le Shell
        await Shell.Current.GoToAsync("RegisterPage");
    }

    [RelayCommand]
    private async Task GoToLogin()
    {
        await Shell.Current.GoToAsync("LoginPage");
        // On pourra créer LoginPage plus tard
        //await Shell.Current.DisplayAlert("Info", "Page de connexion à venir", "OK");
    }
}