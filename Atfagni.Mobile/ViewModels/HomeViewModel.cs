using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Atfagni.Mobile.Services;
using Atfagni.Shared.DTOs;

namespace Atfagni.Mobile.ViewModels;

public partial class HomeViewModel : ObservableObject
{
    private readonly ApiService _apiService;

    // --- Gestion Rôles ---
    [ObservableProperty] bool isDriver;
    [ObservableProperty] bool isPassenger;
    [ObservableProperty] string userName;

    // --- Données Chauffeur ---
    [ObservableProperty] DriverDashboardDto dashboardData;

    // --- Données Passager (Recherche) ---
    [ObservableProperty] string searchFrom;
    [ObservableProperty] string searchTo;
    [ObservableProperty] DateTime searchDate = DateTime.Today;
    public ObservableCollection<RideDto> SearchResults { get; } = new();

    public HomeViewModel(ApiService apiService)
    {
        _apiService = apiService;
        UserName = Preferences.Get("UserName", "Voyageur");

        string role = Preferences.Get("UserRole", "Passenger");
        IsDriver = (role == "Driver");
        IsPassenger = !IsDriver;
    }

    [RelayCommand]
    public async Task Initialize()
    {
        if (IsDriver)
        {
            // Charger le Dashboard (Code précédent)
            string id = Preferences.Get("UserId", "0");
            DashboardData = await _apiService.GetDriverDashboardAsync(int.Parse(id));
        }
        else
        {
            // Pour le passager, on peut charger les trajets récents ou rien du tout
        }
    }

    [RelayCommand]
    public async Task SearchRides()
    {
        if (string.IsNullOrWhiteSpace(SearchFrom) && string.IsNullOrWhiteSpace(SearchTo))
        {
            await Shell.Current.DisplayAlert("Erreur", "Indiquez au moins une ville.", "OK");
            return;
        }

        var rides = await _apiService.SearchRidesAsync(SearchFrom, SearchTo, SearchDate);

        SearchResults.Clear();
        foreach (var r in rides) SearchResults.Add(r);

        if (SearchResults.Count == 0)
            await Shell.Current.DisplayAlertAsync("Info", "Aucun trajet trouvé.", "OK");
    }

    // Commande pour voir le détail (et réserver)
    [RelayCommand]
    async Task GoToRideDetail(RideDto ride)
    {
        // On passera l'objet Ride complet à la page suivante
        var navParam = new Dictionary<string, object> { { "Ride", ride } };
        await Shell.Current.GoToAsync("RideDetailPage", navParam);
    }

    [RelayCommand]
    async Task GoToPublish()
    {
        await Shell.Current.GoToAsync("PublishRidePage");
    }
}