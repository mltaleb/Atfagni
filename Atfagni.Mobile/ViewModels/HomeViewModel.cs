using Atfagni.Mobile.Messages;
using Atfagni.Mobile.Models.Local;
using Atfagni.Mobile.Services;
using Atfagni.Shared.DTOs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;

namespace Atfagni.Mobile.ViewModels;

public partial class HomeViewModel : ObservableObject
{
    private readonly ApiService _apiService;
    private readonly DatabaseService _dbService;

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

    public HomeViewModel(ApiService apiService, DatabaseService dbService)
    {
        _apiService = apiService;
        _dbService = dbService;

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

        _dbService = dbService;
        WeakReferenceMessenger.Default.Register<CitySelectedMessage>(this, (r, m) =>
        {
            if (m.Target == "Departure") SelectedDeparture = m.Value;
            else SelectedArrival = m.Value;
        });
    }
    [RelayCommand]
    private async Task OpenCityPicker(string type)
    {
        try
        {
            // On demande au conteneur de services de nous donner la page proprement
            var page = IPlatformApplication.Current?.Services.GetService<Views.CitySearchPage>();

            if (page == null)
            {
                await Shell.Current.DisplayAlert("Erreur", "La page de recherche n'est pas prête.", "OK");
                return;
            }

            var vm = page.BindingContext as CitySearchViewModel;
            if (vm != null)
            {
                vm.TargetType = type;
            }

            await Shell.Current.Navigation.PushModalAsync(page);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Crash Navigation: {ex.Message}");
        }
    }
    // --- LOGIQUE D'INITIALISATION (Appelée par OnAppearing dans la vue) ---
    [RelayCommand]
    public async Task Initialize()
    {
        if (IsBusy) return; // Sécurité pour éviter les appels en boucle

        try
        {
            // --- ETAPE 1 : CHARGEMENT DU CACHE (Silencieux & Rapide) ---
            if (IsPassenger)
            {
                var cachedRides = await _dbService.GetCachedRidesAsync();
                if (cachedRides.Any())
                {
                    SearchResults.Clear();
                    foreach (var r in cachedRides)
                        SearchResults.Add(r.ToDto());
                }
            }

            // --- ETAPE 2 : CHARGEMENT RÉSEAU (On montre le Spinner) ---
            // On ne lance le réseau que si internet est disponible
            if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
            {
                IsBusy = true; // Ton LoadingOverlay s'affiche ici

                if (IsPassenger)
                {
                    var freshRides =await _apiService.GetLatestRidesAsync(); // Pré-chauffe du cache serveur (optionnel)
                    if (freshRides.Any())
                    {
                        var RidesList = freshRides.Select(r => new LocalRide
                            
                            {
                                Id = r.Id,
                                StartLocation = r.StartLocation,
                                EndLocation = r.EndLocation,
                                Price = r.Price,
                                DepartureTime = r.DepartureTime,
                                DriverName = r.DriverName,
                                AcceptsPackages = r.AcceptsPackages,
                                AvailableSeats = r.AvailableSeats
                            }).ToList();

                        // On rafraîchit les trajets et on met à jour SQLite
                        // On rafraîchit les trajets et on met à jour SQLite
                        await _dbService.SaveRidesCacheAsync(RidesList);

                        SearchResults.Clear();
                        foreach (var r in freshRides)
                            SearchResults.Add(r);

                    }
                    
                }
                else if (IsDriver)
                {
                    // On charge les stats du chauffeur
                    string idStr = Preferences.Get("UserId", "0");
                    var data = await _apiService.GetDriverDashboardAsync(int.Parse(idStr));
                    if (data != null) DashboardData = data;
                }
            }
        }
        catch (Exception ex)
        {
            // En mode PRO, on log l'erreur mais on ne fait pas crasher l'appli
            Console.WriteLine($"Erreur Initialisation: {ex.Message}");
        }
        finally
        {
            // --- ETAPE 3 : FIN DU CHARGEMENT ---
            IsBusy = false; // Le spinner disparaît
        }
        // On ne charge le dashboard que si c'est un chauffeur
        //if (IsDriver)
        //{
        //    try
        //    {
        //        IsBusy = true;
        //        string idStr = Preferences.Get("UserId", "0");
        //        var data = await _apiService.GetDriverDashboardAsync(int.Parse(idStr));
        //        if (data != null)
        //        {
        //            DashboardData = data;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Erreur chargement dashboard: {ex.Message}");
        //    }
        //    finally
        //    {
        //        IsBusy = false;
        //    }
        //}
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
            List<RideDto> results = new();

            // --- CAS 1 : MODE CONNECTÉ (ONLINE) ---
            if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
            {
                var rides = await _apiService.SearchRidesAsync(SelectedDeparture, SelectedArrival, SearchDate);

                if (rides != null && rides.Any())
                {
                    results = rides;
                }
                else
                {
                    // Fallback si pas de résultat direct
                    await Shell.Current.DisplayAlert("Info", "Pas de trajet exact. Voici les dernières nouveautés en cache.", "OK");
                    var cached = await _dbService.GetCachedRidesAsync();
                    results = cached.Select(r => r.ToDto()).ToList();
                }
            }
            // --- CAS 2 : MODE DÉCONNECTÉ (OFFLINE) ---
            else
            {
                // On cherche dans SQLite
                var localMatches = await _dbService.SearchRidesLocalAsync(SelectedDeparture, SelectedArrival);

                if (localMatches.Any())
                {
                    results = localMatches.Select(r => r.ToDto()).ToList();
                }
                else
                {
                    // Si rien en local, on montre tout le cache récent
                    var allCached = await _dbService.GetCachedRidesAsync();
                    results = allCached.Select(r => r.ToDto()).ToList();
                }
            }

            // --- MISE À JOUR UI MODERNE AVEC DISPATCHER ---
            // On attend que l'UI ait fini avant de continuer
            await Shell.Current.Dispatcher.DispatchAsync(() =>
            {
                SearchResults.Clear();
                foreach (var r in results)
                {
                    SearchResults.Add(r);
                }
            });

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur: {ex.Message}");
            await Shell.Current.DisplayAlert("Erreur", "Un problème est survenu.", "OK");
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