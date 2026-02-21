using Atfagni.Mobile.Models.Local;
using Atfagni.Mobile.Services;
using Atfagni.Shared.DTOs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace Atfagni.Mobile.ViewModels;

public partial class HomeViewModel : ObservableObject
{
    private readonly ApiService _apiService;
    private readonly DatabaseService _dbService;

    // --- ÉTAT ET SESSION ---
    [ObservableProperty] private string userName;
    [ObservableProperty] private bool isDriver;
    [ObservableProperty] private bool isPassenger;
    [ObservableProperty] private bool isBusy;

    // --- DONNÉES CHAUFFEUR (Dashboard) ---
    [ObservableProperty] private DriverDashboardDto dashboardData;

    // --- DONNÉES PASSAGER (Recherche) ---
    // Cette liste alimente les deux Pickers (Départ et Arrivée)
    public ObservableCollection<string> CitiesList { get; } = new();
    public ObservableCollection<RideDto> SearchResults { get; } = new();

    [ObservableProperty] private string selectedDeparture;
    [ObservableProperty] private string selectedArrival;
    [ObservableProperty] private DateTime searchDate = DateTime.Today;
    private readonly List<string> _allCities = new()
    {
        "Alger", "Alicante", "Bechar", "Bordeaux", "Ghardaia",
        "Lyon", "Madrid", "Marseille", "Nouadhibou", "Nouakchott",
        "Oran", "Paris", "Rabouni", "Tindouf", "Toulouse", "Zouerate"
    };
    public HomeViewModel(ApiService apiService, DatabaseService dbService)
    {
        _apiService = apiService;
        _dbService = dbService;

        // 1. Charger les infos utilisateur
        UserName = Preferences.Get("UserName", "Voyageur");
        string role = Preferences.Get("UserRole", "Passenger");
        IsDriver = (role == "Driver");
        IsPassenger = !IsDriver;
    }

    // --- LOGIQUE D'INITIALISATION ---
    [RelayCommand]
    public async Task Initialize()
    {
        if (IsBusy) return;

        try
        {
            // --- 1. CHARGEMENT DES VILLES (Une seule fois par session) ---
            // --- 1. CHARGEMENT DES VILLES (SÉCURISÉ) ---
            if (CitiesList.Count == 0)
            {
                // On essaie de lire SQLite
                var citiesFromDb = await _dbService.GetCitiesAsync();

                if (citiesFromDb == null || citiesFromDb.Count == 0)
                {
                    // CAS : BASE VIDE (Premier lancement)
                    // On utilise la liste de secours pour ne pas bloquer l'utilisateur
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        CitiesList.Clear();
                        foreach (var name in _allCities)
                        {
                            CitiesList.Add(name);
                        }
                    });

                    // On lance la synchronisation Cloud -> SQLite en tâche de fond
                    // pour que la base ne soit plus vide au prochain démarrage
                    _ = Task.Run(async () =>
                    {
                        var cloudCities = await _apiService.GetCloudCitiesAsync();
                        if (cloudCities != null && cloudCities.Any())
                        {
                            await _dbService.SyncCitiesFromCloudAsync(cloudCities);
                        }
                        else
                        {
                            // Si même le cloud échoue, on sauve au moins la liste de secours en base
                            await _dbService.SyncCitiesFromCloudAsync(_allCities);
                        }
                    });
                }
                else
                {
                    // CAS : BASE DÉJÀ REMPLIE
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        CitiesList.Clear();
                        foreach (var c in citiesFromDb)
                        {
                            CitiesList.Add(c.Name);
                        }
                    });
                }
            }

            // --- 2. AFFICHAGE INSTANTANÉ DU CACHE (Toujours) ---
            if (IsPassenger)
            {
                var cachedRides = await _dbService.GetCachedRidesAsync();
                if (cachedRides.Any())
                {
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        SearchResults.Clear();
                        foreach (var r in cachedRides) SearchResults.Add(r.ToDto());
                    });
                }
            }

            // --- 3. DÉCISION D'APPELER LE SERVEUR (Logique intelligente) ---
            if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
            {
                // Récupérer l'heure de la dernière mise à jour
                if (IsPassenger)
                {
                    var lastUpdate = Preferences.Get("LastRidesUpdate", DateTime.MinValue);
                    var cacheDuration = DateTime.Now - lastUpdate;

                    // Rafraîchir si > 5 minutes ou si l'écran est vide
                    if (cacheDuration.TotalMinutes > 5 || SearchResults.Count == 0)
                    {
                        await RefreshDataFromServer();
                    }
                }
                else if (IsDriver)
                {
                    // --- AJOUT TTL DASHBOARD CHAUFFEUR ---
                    var lastUpdate = Preferences.Get("LastDashboardUpdate", DateTime.MinValue);
                    var cacheDuration = DateTime.Now - lastUpdate;

                    // Pour le chauffeur, on rafraîchit toutes les 2 minutes (plus fréquent pour le solde)
                    // ou si les données n'ont jamais été chargées
                    if (cacheDuration.TotalMinutes > 2 || DashboardData == null)
                    {
                        await RefreshDataFromServer();
                    }
                }

            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur Init: {ex.Message}");
        }
        finally { IsBusy = false; }
    }

    // Méthode séparée pour la clarté
   private async Task RefreshDataFromServer()
{
    // LOGIQUE PRO : On ne montre le spinner bloquant QUE pour le passager.
    // Pour le chauffeur, la mise à jour sera invisible (Silent Sync).
    bool showSpinner = IsPassenger; 

    try
    {
        if (showSpinner) IsBusy = true;

        if (IsPassenger)
        {
            var freshRides = await _apiService.GetLatestRidesAsync();
            if (freshRides != null)
            {
                // Sauvegarde SQLite
                await _dbService.SaveRidesCacheAsync(freshRides.Select(r => new LocalRide
                {
                    Id = r.Id,
                    StartLocation = r.StartLocation,
                    EndLocation = r.EndLocation,
                    Price = r.Price,
                    DepartureTime = r.DepartureTime,
                    DriverName = r.DriverName,
                    AcceptsPackages = r.AcceptsPackages,
                    AvailableSeats = r.AvailableSeats
                }).ToList());

                // Mise à jour UI (Thread Safe)
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    SearchResults.Clear();
                    foreach (var r in freshRides) SearchResults.Add(r);
                });

                Preferences.Set("LastRidesUpdate", DateTime.Now);
            }
        }
        else if (IsDriver)
        {
            // --- MISE À JOUR SILENCIEUSE POUR LE CHAUFFEUR ---
            string idStr = Preferences.Get("UserId", "0");
            var data = await _apiService.GetDriverDashboardAsync(int.Parse(idStr));
            
            if (data != null) 
            {
                // On met à jour les données. Comme IsBusy est false, 
                // les chiffres changent à l'écran sans bloquer l'utilisateur.
                DashboardData = data;
                
                // On enregistre aussi l'heure pour le chauffeur
                Preferences.Set("LastDashboardUpdate", DateTime.Now);
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erreur Refresh: {ex.Message}");
    }
    finally 
    { 
        if (showSpinner) IsBusy = false; 
    }
}
    // --- RECHERCHE ---
    [RelayCommand]
    public async Task SearchRides()
    {
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
            List<RideDto> results = new();

            if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
            {
                var rides = await _apiService.SearchRidesAsync(SelectedDeparture, SelectedArrival, SearchDate);
                if (rides != null && rides.Any()) results = rides;
                else
                {
                    await Shell.Current.DisplayAlert("Info", "Pas de trajet exact. Voici le cache.", "OK");
                    var cached = await _dbService.GetCachedRidesAsync();
                    results = cached.Select(r => r.ToDto()).ToList();
                }
            }
            else
            {
                var localMatches = await _dbService.SearchRidesLocalAsync(SelectedDeparture, SelectedArrival);
                results = localMatches.Any()
                    ? localMatches.Select(r => r.ToDto()).ToList()
                    : (await _dbService.GetCachedRidesAsync()).Select(r => r.ToDto()).ToList();
            }

            await Shell.Current.Dispatcher.DispatchAsync(() =>
            {
                SearchResults.Clear();
                foreach (var r in results) SearchResults.Add(r);
            });

        }
        catch (Exception ex) { Console.WriteLine(ex.Message); }
        finally { IsBusy = false; }
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