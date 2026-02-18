using Atfagni.Mobile.Services;
using Atfagni.Shared.DTOs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace Atfagni.Mobile.ViewModels.Rides;

public partial class BookingHistoryViewModel : ObservableObject
{
    private readonly ApiService _apiService;

    // La liste qui sera liée à la CollectionView
    public ObservableCollection<BookingRequestDto> HistoryList { get; } = new();

    [ObservableProperty]
    private bool isBusy;

    public BookingHistoryViewModel(ApiService apiService)
    {
        _apiService = apiService;
    }

    [RelayCommand]
    public async Task LoadHistory()
    {
        if (IsBusy) return;

        IsBusy = true;

        // 1. Récupérer l'ID du chauffeur
        string driverIdStr = Preferences.Get("UserId", "0");
        int driverId = int.Parse(driverIdStr);

        // 2. Appel Service
        var bookings = await _apiService.GetBookingHistoryAsync(driverId);

        // 3. Mise à jour de l'interface
        HistoryList.Clear();
        foreach (var booking in bookings)
        {
            // Petite astuce visuelle : Si le statut n'est pas dans le DTO, 
            // on peut utiliser PackageDescription pour l'affichage temporaire 
            // ou s'assurer que le backend renvoie bien le statut.
            HistoryList.Add(booking);
        }

        IsBusy = false;
    }
}