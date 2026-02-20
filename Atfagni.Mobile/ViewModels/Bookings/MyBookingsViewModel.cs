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
    private void UpdateList(List<LocalBooking> list)
    {
        UpcomingBookings.Clear();
        foreach (var b in list)
        {
            UpcomingBookings.Add(b.ToDto());
        }
    }
    [RelayCommand]
    public async Task LoadBookings()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            string userIdStr = Preferences.Get("UserId", "0");
            int userId = int.Parse(userIdStr);

            // --- 1. CHARGER LE CACHE (Instantané) ---
            var cached = await _dbService.GetBookingsAsync();
            if (cached != null && cached.Any())
            {
                UpdateList(cached);
            }

            // --- 2. APPEL RÉSEAU (Si internet dispo) ---
            if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
            {
                var freshBookings = await _apiService.GetPassengerBookingsAsync(userId);

                if (freshBookings != null)
                {
                    // Transformer les DTOs reçus en Modèles Locaux pour SQLite
                    var localList = freshBookings.Select(b => new LocalBooking
                    {
                        Id = b.Id,
                        DriverName = b.PassengerName, // Rappel: contient le nom du chauffeur
                        DriverPhone = b.PassengerPhone,
                        TripDescription = b.TripDescription,
                        Status = b.Status.ToString(),
                        PackageDescription = b.PackageDescription
                    }).ToList();

                    // --- 3. SAUVEGARDER DANS SQLITE ---
                    await _dbService.SaveBookingsAsync(localList);

                    // --- 4. METTRE À JOUR L'ÉCRAN AVEC LES DONNÉES FRAICHES ---
                    UpdateList(localList);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur LoadBookings: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }
}