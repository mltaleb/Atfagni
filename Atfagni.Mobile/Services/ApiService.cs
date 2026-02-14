using Atfagni.Shared.DTOs;
using System.Diagnostics;
using System.Net.Http.Json;

namespace Atfagni.Mobile.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;
    private string BaseUrl;

    public ApiService()
    {
       
            BaseUrl = "http://localhost:5077"; // Émulateur Android
        
       
        _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
    }

    // --- LOGIN ---
    public async Task<UserDto?> LoginAsync(LoginRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/users/login", request);

            if (response.IsSuccessStatusCode)
            {
                // On déserialize la réponse JSON en objet UserDto
                return await response.Content.ReadFromJsonAsync<UserDto>();
            }

            return null; // Identifiants incorrects
        }
        catch (Exception ex)
        {
            // Erreur de connexion (Serveur éteint, pas d'internet...)
            Debug.WriteLine($"Erreur Login: {ex.Message}");
            return null;
        }
    }

    // --- REGISTER ---
    public async Task<bool> RegisterAsync(RegisterRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/users/register", request);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
    public async Task<bool> BookRideAsync(CreateBookingRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/bookings/reserve", request);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur Reservation: {ex.Message}");
            return false;
        }
    }

    // 2. Récupérer les demandes en attente (Chauffeur)
    public async Task<List<BookingRequestDto>> GetPendingRequestsAsync(int driverId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<BookingRequestDto>>($"/api/bookings/driver/{driverId}/pending")
                   ?? new List<BookingRequestDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur Pending Requests: {ex.Message}");
            return new List<BookingRequestDto>();
        }
    }

    // 3. Répondre à une demande (Chauffeur)
    public async Task<bool> RespondToBookingAsync(int bookingId, bool accept)
    {
        try
        {
            // On envoie le booléen 'accept' en query parameter string pour faire simple, 
            // ou on peut le mettre dans le body. Ici, l'API attendait "/decide?accept=true"
            var response = await _httpClient.PostAsync($"/api/bookings/{bookingId}/decide?accept={accept}", null);

            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur Decision: {ex.Message}");
            return false;
        }
    }
    public async Task<bool> PublishRideAsync(CreateRideRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/rides", request);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    // Récupérer les trajets d'un chauffeur spécifique
    public async Task<List<RideDto>> GetMyRidesAsync(int driverId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<RideDto>>($"/api/rides/driver/{driverId}")
                   ?? new List<RideDto>();
        }
        catch { return new List<RideDto>(); }
    }

    // Supprimer (Annuler) un trajet
    public async Task<bool> DeleteRideAsync(int rideId)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/api/rides/{rideId}");
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    // Mettre à jour un trajet (Optionnel pour l'instant si on fait juste suppression)
    public async Task<bool> UpdateRideAsync(int rideId, UpdateRideRequest request)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/rides/{rideId}", request);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }


}