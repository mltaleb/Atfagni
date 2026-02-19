using Atfagni.Mobile.Services;
using Atfagni.Shared.DTOs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace Atfagni.Mobile.ViewModels.Bookings;

public partial class MyBookingsViewModel : ObservableObject
{
    private readonly ApiService _apiService;

    [ObservableProperty] bool isBusy;

    public ObservableCollection<BookingRequestDto> UpcomingBookings { get; } = new();
    public ObservableCollection<BookingRequestDto> PastBookings { get; } = new();

    public MyBookingsViewModel(ApiService apiService) => _apiService = apiService;

    [RelayCommand]
    public async Task LoadBookings()
    {
        // Sécurité : si on charge déjà, on ignore le deuxième appel
        if (IsBusy) return;

        try
        {
            IsBusy = true; // Affiche le spinner

            string userIdStr = Preferences.Get("UserId", "0");
            int userId = int.Parse(userIdStr);

            var allBookings = await _apiService.GetPassengerBookingsAsync(userId);

            UpcomingBookings.Clear();
            PastBookings.Clear();

            if (allBookings != null)
            {
                foreach (var b in allBookings)
                {
                    // Ici tu pourras ajouter ta logique de tri par date
                    // Ex: if(b.RideDate > DateTime.Now) Upcoming... else Past...
                    UpcomingBookings.Add(b);
                }
            }
        }
        catch (Exception ex)
        {
            // Gérer l'erreur proprement (ex: serveur Render endormi)
            Console.WriteLine($"Erreur chargement réservations: {ex.Message}");
            // Optionnel : await Shell.Current.DisplayAlert("Erreur", "Impossible de charger vos réservations", "OK");
        }
        finally
        {
            IsBusy = false; // Cache le spinner quoi qu'il arrive
        }
    }
}