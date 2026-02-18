using Atfagni.Mobile.Services;
using Atfagni.Shared.DTOs;
using Atfagni.Shared.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Atfagni.Mobile.ViewModels.Rides;

[QueryProperty(nameof(Ride), "Ride")] // Reçoit le trajet par navigation
public partial class RideDetailViewModel : ObservableObject
{
    private readonly ApiService _apiService;

    [ObservableProperty] RideDto ride;
    [ObservableProperty] bool isBusy;
    [ObservableProperty] int seatsToBook = 1;

    public RideDetailViewModel(ApiService apiService) => _apiService = apiService;

    [RelayCommand]
    async Task ConfirmBooking()
    {
        bool confirm = await Shell.Current.DisplayAlert("Confirmation",
            $"Réserver {SeatsToBook} place(s) pour {Ride.Price * SeatsToBook} DA ?", "Oui", "Annuler");

        if (!confirm) return;

        try
        {
            IsBusy = true;
            var request = new CreateBookingRequest
            {
                RideId = Ride.Id,
                PassengerId = int.Parse(Preferences.Get("UserId", "0")),
                Seats = SeatsToBook,
                Type = BookingType.Passenger
            };

            bool success = await _apiService.BookRideAsync(request);
            if (success)
            {
                await Shell.Current.DisplayAlert("Succès", "Votre demande a été envoyée au chauffeur !", "OK");
                await Shell.Current.GoToAsync(".."); // Retour à l'accueil
            }
        }
        finally { IsBusy = false; }
    }
}