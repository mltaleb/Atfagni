using Atfagni.Mobile.Services;
using Atfagni.Shared.DTOs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace Atfagni.Mobile.ViewModels;

public partial class HomeViewModel : ObservableObject
{
    private readonly ApiService _apiService;

    // --- LISTE MAÎTRE DES VILLES ---
    private readonly List<string> _allCities = new()
    {
        "Alger", "Alicante", "Bechar", "Bordeaux", "Madrid",
        "Marseille", "Nouakchott", "Oran", "Paris", "Tindouf", "Zouerate"
    };

    // --- ÉTAT ET SESSION ---
    [ObservableProperty] private string userName;
    [ObservableProperty] private bool isDriver;
    [ObservableProperty] private bool isPassenger;
    [ObservableProperty] private bool isBusy;

    // --- DONNÉES CHAUFFEUR (Dashboard) ---
    [ObservableProperty] private DriverDashboardDto dashboardData;

    // --- DONNÉES PASSAGER (Recherche) ---
    public ObservableCollection<string> DepartureCities { get; } = new();
    public ObservableCollection<string> ArrivalCities { get; } = new();
    public ObservableCollection<RideDto> SearchResults { get; } = new();

    [ObservableProperty] private string selectedDeparture;
    [ObservableProperty] private string selectedArrival;
    [ObservableProperty] private DateTime searchDate = DateTime.Today;

    public HomeViewModel(ApiService apiService)
    {
        _apiService = apiService;

        // 1. Charger les infos de l'utilisateur stockées au Login
        UserName = Preferences.Get("UserName", "Voyageur");
        string role = Preferences.Get("UserRole", "Passenger");
        IsDriver = (role == "Driver");
        IsPassenger = !IsDriver;

        // 2. Initialiser les listes des Pickers (Une seule fois pour éviter les crashs)
        foreach (var c in _allCities)
        {
            DepartureCities.Add(c);
            ArrivalCities.Add(c);
        }
    }

    // --- LOGIQUE D'INITIALISATION (Appelée par OnAppearing dans la vue) ---
    [RelayCommand]
    public async Task Initialize()
    {
       
        // On ne charge le dashboard que si c'est un chauffeur
        if (IsDriver)
        {
            try
            {
                IsBusy = true;
                string idStr = Preferences.Get("UserId", "0");
                var data = await _apiService.GetDriverDashboardAsync(int.Parse(idStr));
                if (data != null)
                {
                    DashboardData = data;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur chargement dashboard: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }

    // --- COMMANDES DE RECHERCHE (Logique Simplifiée et Stable) ---

    [RelayCommand]
    public async Task SearchRides()
    {
        // 1. Validation de base
        if (string.IsNullOrEmpty(SelectedDeparture) || string.IsNullOrEmpty(SelectedArrival))
        {
            await Shell.Current.DisplayAlert("Atfagni", "Veuillez choisir le départ et l'arrivée.", "OK");
            return;
        }

        if (SelectedDeparture == SelectedArrival)
        {
            await Shell.Current.DisplayAlert("Attention", "Le départ et l'arrivée sont identiques.", "OK");
            return;
        }

        try
        {
            IsBusy = true;

            // 2. Appel de la recherche
            // Note : Ton API actuelle fait sans doute un "ET". 
            // Si tu veux un "OU", il faudra modifier le code du Backend dans /search.
            var rides = await _apiService.SearchRidesAsync(SelectedDeparture, SelectedArrival, SearchDate);

            SearchResults.Clear();

            // 3. LOGIQUE DE REPLI (Fallback)
            if (rides == null || rides.Count == 0)
            {
                // On prévient l'utilisateur
                await Shell.Current.DisplayAlert("Aucun résultat direct",
                    "Nous n'avons pas trouvé de trajet exact. Voici les dernières publications.", "Voir");

                // On récupère les tout derniers trajets publiés
                var latest = await _apiService.GetLatestRidesAsync();
                foreach (var r in latest)
                {
                    SearchResults.Add(r);
                }
            }
            else
            {
                // On affiche les résultats trouvés
                foreach (var r in rides)
                {
                    SearchResults.Add(r);
                }
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Erreur", "Serveur injoignable.", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    // --- NAVIGATION ---

    [RelayCommand]
    async Task GoToRideDetail(RideDto ride)
    {
        if (ride == null) return;
        var navParam = new Dictionary<string, object> { { "Ride", ride } };
        await Shell.Current.GoToAsync("RideDetailPage", navParam);
    }

    [RelayCommand]
    async Task GoToPublish() => await Shell.Current.GoToAsync("PublishRidePage");

    [RelayCommand]
    async Task GoToRequests() => await Shell.Current.GoToAsync("DriverRequestsPage");

    [RelayCommand]
    async Task GoToHistory() => await Shell.Current.GoToAsync("BookingHistoryPage");
}