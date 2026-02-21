using Atfagni.Mobile.Models.Local;
using Atfagni.Mobile.Services;
using Atfagni.Shared.DTOs;
using Atfagni.Shared.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace Atfagni.Mobile.ViewModels.Bookings;

public partial class MyBookingsViewModel : ObservableObject
{
    private readonly ApiService _apiService;
    private readonly DatabaseService _dbService;

    [ObservableProperty] bool isBusy;

    public ObservableCollection<BookingRequestDto> UpcomingBookings { get; } = new();
    public ObservableCollection<BookingRequestDto> PastBookings { get; } = new();

    public MyBookingsViewModel(ApiService apiService, DatabaseService dbService)
    {
        _apiService = apiService;
        _dbService = dbService;
    }
  

    [RelayCommand]
    public async Task LoadBookings()
    {
        if (IsBusy) return;

        try
        {
            // --- 1. CHARGER LE CACHE (Instantané - Sans Spinner) ---
            // On montre ce qu'on a déjà en mémoire pour que l'utilisateur ne voit pas un écran vide
            var cached = await _dbService.GetBookingsAsync();
            if (cached != null && cached.Any())
            {
                UpdateList(cached);
            }

            // --- 2. DÉCISION D'APPEL RÉSEAU (Logique 5 minutes) ---
            if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
            {
                var lastUpdate = Preferences.Get("LastBookingsUpdate", DateTime.MinValue);
                var elapsed = DateTime.Now - lastUpdate;

                // On ne rafraîchit que si > 5 min OU si on n'a absolument rien à afficher
                if (elapsed.TotalMinutes > 5 || (cached == null || !cached.Any()))
                {
                    await RefreshBookingsFromServer();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur LoadBookings: {ex.Message}");
        }
        // Note: IsBusy est géré dans RefreshBookingsFromServer ou mis à false ici
        finally { IsBusy = false; }
    }

    // Méthode dédiée pour la mise à jour réseau
    private async Task RefreshBookingsFromServer()
    {
        try
        {
            IsBusy = true; // On affiche le spinner seulement pendant l'appel réseau

            string userIdStr = Preferences.Get("UserId", "0");
            int userId = int.Parse(userIdStr);

            var freshBookings = await _apiService.GetPassengerBookingsAsync(userId);

            if (freshBookings != null)
            {
                // Conversion pour SQLite
                var localList = freshBookings.Select(b => new LocalBooking
                {
                    Id = b.Id,
                    DriverName = b.PassengerName,
                    DriverPhone = b.PassengerPhone,
                    TripDescription = b.TripDescription,
                    Status = b.Status.ToString(),
                    PackageDescription = b.PackageDescription,
                    DepartureDate = b.DepartureDate
                }).ToList();

                // Sauvegarde dans SQLite
                await _dbService.SaveBookingsAsync(localList);

                // Mise à jour de l'heure du cache
                Preferences.Set("LastBookingsUpdate", DateTime.Now);

                // Mise à jour UI
                UpdateList(localList);
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void UpdateList(List<LocalBooking> list)
    {
        // Sécurité Thread UI pour MAUI
        MainThread.BeginInvokeOnMainThread(() =>
        {
            UpcomingBookings.Clear();
            PastBookings.Clear();

            foreach (var b in list)
            {
                var dto = b.ToDto();

                // Tri simple : Si c'est Terminé ou Annulé -> Passé, sinon -> À venir
                if (dto.Status == BookingStatus.Completed || dto.Status == BookingStatus.Cancelled || dto.Status == BookingStatus.Rejected)
                {
                    PastBookings.Add(dto);
                }
                else
                {
                    UpcomingBookings.Add(dto);
                }
            }
        });
    }
    [RelayCommand]
    private async Task CallDriver(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber) || phoneNumber == "Numéro masqué")
            return;

        try
        {
            if (PhoneDialer.Default.IsSupported)
                PhoneDialer.Default.Open(phoneNumber);
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Erreur", "Impossible d'ouvrir le téléphone.", "OK");
        }
    }
}