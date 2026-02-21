using Atfagni.Mobile.Models.Local;
using Atfagni.Mobile.Services;
using Atfagni.Shared.DTOs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace Atfagni.Mobile.ViewModels.Rides;

public partial class MyRidesViewModel : ObservableObject
{
    private readonly ApiService _apiService;
    private readonly DatabaseService _dbService;
    public ObservableCollection<RideDto> Rides { get; } = new();
    [ObservableProperty] private bool isBusy;
    public MyRidesViewModel(ApiService apiService, DatabaseService dbService)
    {
        _apiService = apiService;
        _dbService = dbService;

    }
    private void UpdateUI(List<LocalPublishedRide> localRides)
    {
        // Sécurité obligatoire pour Android : on touche à la liste sur le thread principal
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Rides.Clear();

            if (localRides != null)
            {
                foreach (var ride in localRides)
                {
                    // On transforme chaque ligne SQLite en DTO pour l'affichage
                    Rides.Add(ride.ToDto());
                }
            }

            Console.WriteLine($"[DEBUG] Liste 'Mes Trajets' mise à jour : {Rides.Count} éléments.");
        });
    }
    [RelayCommand]
    public async Task LoadRides()
    {
        // On ne met PAS IsBusy = true ici pour éviter le spinner au retour de navigation
        try
        {
            // 1. CHARGER LE CACHE (Instantané, pas de spinner)
            var cached = await _dbService.GetMyRidesAsync();
            if (cached.Any())
            {
                UpdateUI(cached);
            }
            else
            {
                // SI LA LISTE EST VIDE : On affiche le gros LoadingOverlay
                IsBusy = true;
            }

            // 2. LOGIQUE DE DÉCISION RÉSEAU
            if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
            {
                var lastUpdate = Preferences.Get("LastMyRidesUpdate", DateTime.MinValue);
                bool isOld = (DateTime.Now - lastUpdate).TotalMinutes > 5;

                // On ne lance l'appel serveur que si c'est vieux ou si le cache est vide
                if (isOld || !cached.Any())
                {
                    await RefreshMyRidesFromServer();
                }
            }
        }
        catch (Exception ex) { Console.WriteLine(ex.Message); }
        finally
        {
            IsBusy = false; // Arrête le spinner quel qu'il soit
        }
    }

    private async Task RefreshMyRidesFromServer()
    {
        var driverId = int.Parse(Preferences.Get("UserId", "0"));
        var fresh = await _apiService.GetMyRidesAsync(driverId);

        if (fresh != null)
        {
            var locals = fresh.Select(r => new LocalPublishedRide
            {
                Id = r.Id,
                StartLocation = r.StartLocation,
                EndLocation = r.EndLocation,
                Price = r.Price,
                DepartureTime = r.DepartureTime,
                AvailableSeats = r.AvailableSeats,
                Status = r.Status.ToString()
            }).ToList();

            await _dbService.SaveMyRidesAsync(locals);
            Preferences.Set("LastMyRidesUpdate", DateTime.Now);
            UpdateUI(locals);
        }
    }





    [RelayCommand]
    public async Task DeleteRide(RideDto ride)
    {
        bool confirm = await Shell.Current.DisplayAlertAsync("Attention", "Voulez-vous vraiment annuler ce trajet ?", "Oui", "Non");
        if (!confirm) return;

        bool success = await _apiService.DeleteRideAsync(ride.Id);
        if (success)
        {
            Rides.Remove(ride); // On retire l'élément de la liste sans recharger
            await Shell.Current.DisplayAlertAsync("Info", "Trajet annulé.", "OK");
        }
        else
        {
            await Shell.Current.DisplayAlertAsync("Erreur", "Impossible d'annuler.", "OK");
        }
    }

    [RelayCommand]
    public async Task EditRide(RideDto ride)
    {
        await Shell.Current.DisplayAlertAsync("Info", "La modification sera disponible dans la prochaine version. Veuillez supprimer et recréer si besoin.", "OK");
        // Pour faire l'edit, il faudrait naviguer vers PublishRidePage avec les données pré-remplies.
    }
    [RelayCommand]
    private async Task GoToPublish()
    {
        // On navigue vers la page de publication
        await Shell.Current.GoToAsync("PublishRidePage");
    }
}