using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Atfagni.Mobile.Services;
using Atfagni.Shared.DTOs; // Important !

namespace Atfagni.Mobile.ViewModels.Bookings;

public partial class DriverRequestsViewModel : ObservableObject
{
    private readonly ApiService _apiService;

    // On utilise le DTO partagé ici
    public ObservableCollection<BookingRequestDto> Requests { get; } = new();

    public DriverRequestsViewModel(ApiService apiService)
    {
        _apiService = apiService;
    }

    [RelayCommand]
    public async Task LoadRequests()
    {
        string driverId = Preferences.Get("UserId", "0");
        if (driverId == "0") return;

        var list = await _apiService.GetPendingRequestsAsync(int.Parse(driverId));

        Requests.Clear();
        foreach (var req in list)
        {
            Requests.Add(req);
        }
    }

    [RelayCommand]
    public async Task Accept(BookingRequestDto req)
    {
        bool success = await _apiService.RespondToBookingAsync(req.Id, true);
        if (success)
        {
            Requests.Remove(req);
            await Shell.Current.DisplayAlert("Succès", $"Réservation de {req.PassengerName} acceptée !", "OK");
        }
    }

    [RelayCommand]
    public async Task Reject(BookingRequestDto req)
    {
        bool success = await _apiService.RespondToBookingAsync(req.Id, false);
        if (success)
        {
            Requests.Remove(req);
            await Shell.Current.DisplayAlert("Info", "Réservation refusée.", "OK");
        }
    }
}