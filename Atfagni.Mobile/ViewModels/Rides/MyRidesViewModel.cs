using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Atfagni.Mobile.Services;
using Atfagni.Shared.DTOs;

namespace Atfagni.Mobile.ViewModels.Rides;

public partial class MyRidesViewModel : ObservableObject
{
    private readonly ApiService _apiService;
    public ObservableCollection<RideDto> Rides { get; } = new();

    public MyRidesViewModel(ApiService apiService)
    {
        _apiService = apiService;
         
    }

    [RelayCommand]
    public async Task LoadRides()
    {
        string driverId = Preferences.Get("UserId", "0");
        var list = await _apiService.GetMyRidesAsync(int.Parse(driverId));

        Rides.Clear();
        foreach (var ride in list) Rides.Add(ride);
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