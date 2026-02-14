using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Atfagni.Mobile.ViewModels;

public partial class HomeViewModel : ObservableObject
{
    [ObservableProperty]
    private string welcomeMessage;

    [ObservableProperty]
    private bool isDriver;

    [ObservableProperty]
    private bool isPassenger;
    [ObservableProperty] private string driverId;

    public HomeViewModel()
    {
        // On récupère les infos sauvegardées lors du Login
        string role = Preferences.Get("UserRole", "Passenger");
        string name = Preferences.Get("UserName", "Voyageur");
        string _driverId = Preferences.Get("UserId", "0");

        WelcomeMessage = $"Bonjour, {name}";

        // On définit les booléens pour le Binding IsVisible du XAML
        IsDriver = (role == "Driver");
        IsPassenger = (role == "Passenger");
        DriverId = _driverId;
    }
    [RelayCommand]
    private async Task Logout()
    {
        // On efface tout ce qui est stocké
        Preferences.Clear();

        // On peut aussi utiliser Preferences.Remove("UserId") si on veut garder certains réglages

        // On renvoie l'utilisateur à l'écran de Bienvenue
        await Shell.Current.GoToAsync("MainPage");
    }
    [RelayCommand]
    async Task GoToPublishRide()
    {
        await Shell.Current.GoToAsync("PublishRidePage");
    }
    [RelayCommand]
    async Task GoToMyRides()
    {
        await Shell.Current.GoToAsync("MyRidesPage");
    }
}