using System.Text.Json;
using Atfagni.Mobile.Services;

namespace Atfagni.Mobile.Services;

public class CityService
{
    private readonly ApiService _apiService;
    private List<string> _cachedCities;

    public CityService(ApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<List<string>> GetCitiesAsync()
    {
        // 1. Si déjà chargé en mémoire pendant cette session
        if (_cachedCities != null && _cachedCities.Any())
            return _cachedCities;

        // 2. Sinon, regarder dans le stockage local (Preferences)
        string savedCitiesJson = Preferences.Get("cached_cities", string.Empty);
        if (!string.IsNullOrEmpty(savedCitiesJson))
        {
            _cachedCities = JsonSerializer.Deserialize<List<string>>(savedCitiesJson);

            // On lance quand même un rafraîchissement silencieux en arrière-plan pour la prochaine fois
            _ = RefreshCacheAsync();

            return _cachedCities;
        }

        // 3. Sinon (premier lancement), forcer le téléchargement
        return await RefreshCacheAsync();
    }

    private async Task<List<string>> RefreshCacheAsync()
    {
        try
        {
            var cities = await _apiService.GetCitiesAsync(); // Appelle l'API
            if (cities != null && cities.Any())
            {
                _cachedCities = cities;
                // Sauvegarde locale pour le mode hors-ligne / démarrage rapide
                Preferences.Set("cached_cities", JsonSerializer.Serialize(cities));
                return _cachedCities;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur rafraichissement villes: {ex.Message}");
        }

        return _cachedCities ?? new List<string>();
    }
}