using Atfagni.Mobile.Services;
using Atfagni.Shared.DTOs; // Important !
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Atfagni.Mobile.Models.Local;

namespace Atfagni.Mobile.ViewModels.Bookings;

public partial class DriverRequestsViewModel : ObservableObject
{
    private readonly ApiService _apiService;
    private readonly DatabaseService _dbService;

    public ObservableCollection<BookingRequestDto> Requests { get; } = new();
    [ObservableProperty] bool isBusy;

    public DriverRequestsViewModel(ApiService api, DatabaseService db)
    {
        _apiService = api;
        _dbService = db;
    }

    [RelayCommand]
    public async Task LoadRequests()
    {
        try
        {
            // 1. CHARGER LE CACHE SQLITE
            var cached = await _dbService.GetDriverRequestsAsync();
            UpdateUI(cached);

            // 2. LOGIQUE RÉSEAU (Si > 2 min ou vide)
            if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
            {
                var lastUpdate = Preferences.Get("LastRequestsUpdate", DateTime.MinValue);
                if ((DateTime.Now - lastUpdate).TotalMinutes > 2 || !Requests.Any())
                {
                    await RefreshRequestsFromServer();
                }
            }
        }
        finally { IsBusy = false; }
    }

    private async Task RefreshRequestsFromServer()
    {
        IsBusy = true;
        int driverId = int.Parse(Preferences.Get("UserId", "0"));
        var fresh = await _apiService.GetPendingRequestsAsync(driverId);

        if (fresh != null)
        {
            // Sauvegarde SQLite
            var locals = fresh.Select(r => new LocalDriverRequest
            {
                Id = r.Id,
                PassengerName = r.PassengerName,
                PassengerPhone = r.PassengerPhone,
                TripDescription = r.TripDescription,
                Type = r.Type,
                PackageDescription = r.PackageDescription,
                SeatsRequested = r.SeatsRequested
            }).ToList();

            await _dbService.SaveDriverRequestsAsync(locals);
            Preferences.Set("LastRequestsUpdate", DateTime.Now);
            UpdateUI(locals);
        }
        IsBusy = false;
    }

    private void UpdateUI(List<LocalDriverRequest> list)
    {
        MainThread.BeginInvokeOnMainThread(() => {
            Requests.Clear();
            foreach (var r in list)
                Requests.Add(new BookingRequestDto
                {
                    Id = r.Id,
                    PassengerName = r.PassengerName,
                    TripDescription = r.TripDescription,
                    Type = r.Type,
                    PackageDescription = r.PackageDescription,
                    SeatsRequested = r.SeatsRequested
                });
        });
    }

    [RelayCommand]
    async Task Accept(BookingRequestDto req) => await Respond(req, true);

    [RelayCommand]
    async Task Reject(BookingRequestDto req) => await Respond(req, false);

    private async Task Respond(BookingRequestDto req, bool accept)
    {
        if (IsBusy) return;
        try
        {
            IsBusy = true;
            bool success = await _apiService.RespondToBookingAsync(req.Id, accept);
            if (success)
            {
                Requests.Remove(req);
                // On met à jour le cache SQLite aussi
                await _dbService.DeleteLocalRequestAsync(req.Id);
                await Shell.Current.DisplayAlert("Succès", accept ? "Accepté" : "Refusé", "OK");
            }
        }
        finally { IsBusy = false; }
    }
}