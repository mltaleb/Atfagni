using Atfagni.Mobile.Services;
using Atfagni.Shared.DTOs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Security.Cryptography.X509Certificates;

namespace Atfagni.Mobile.ViewModels.Rides;

public partial class PublishRideViewModel : ObservableObject
{
    private readonly ApiService _apiService;

    public PublishRideViewModel(ApiService apiService)
    {
        _apiService = apiService;
        // Date par défaut = Demain 8h00
        DepartureDate = DateTime.Today.AddDays(1);
        DepartureTime = new TimeSpan(8, 0, 0);
    }

    [ObservableProperty] string startLocation;
    [ObservableProperty] string endLocation;

    [ObservableProperty] DateTime departureDate;
    [ObservableProperty] TimeSpan departureTime; // MAUI sépare Date et Heure

    [ObservableProperty] decimal pricePerSeat;
    [ObservableProperty] int totalSeats = 4;

    // --- Gestion Mixte ---
    [ObservableProperty] bool acceptsPassengers = true;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ShowPackageOptions))] // Met à jour l'interface si ça change
    bool acceptsPackages = false;

    [ObservableProperty] decimal? pricePerPackage;
    [ObservableProperty] string packageDescription;

    public bool ShowPackageOptions => AcceptsPackages;

    [RelayCommand]
    async Task Publish()
    {
        if (string.IsNullOrWhiteSpace(StartLocation) || string.IsNullOrWhiteSpace(EndLocation))
        {
            await Shell.Current.DisplayAlertAsync("Erreur", "Veuillez indiquer le départ et l'arrivée.", "OK");
            return;
        }

        // Récupérer l'ID chauffeur
        string driverId = Preferences.Get("UserId", "0");

        // Combiner Date et Heure
        DateTime fullDate = DepartureDate.Date + DepartureTime;

        var request = new CreateRideRequest
        {
            DriverId = int.Parse(driverId),
            StartLocation = StartLocation,
            EndLocation = EndLocation,
            DepartureTime = fullDate, // Envoyer en format complet
            PricePerSeat = PricePerSeat,
            TotalSeats = TotalSeats,
            AcceptsPassengers = AcceptsPassengers,
            AcceptsPackages = AcceptsPackages,
            PricePerPackage = AcceptsPackages ? PricePerPackage : null,
            PackageDescription = AcceptsPackages ? PackageDescription : null
        };

        bool success = await _apiService.PublishRideAsync(request);

        if (success)
        {
            await Shell.Current.DisplayAlertAsync("Succès", "Trajet publié !", "OK");
            // Retour à l'accueil
            await Shell.Current.GoToAsync("..");
        }
        else
        {
            await Shell.Current.DisplayAlertAsync("Erreur", "Impossible de publier le trajet.", "OK");
        }
    }
}