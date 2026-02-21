using Atfagni.Mobile.Models.Local;
using SQLite;

namespace Atfagni.Mobile.Services;

public class DatabaseService
{
    private SQLiteAsyncConnection _database;

    private async Task Init()
    {
        if (_database is not null) return;

        // Définir le chemin du fichier sur le téléphone
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "AtfagniLocal.db3");

        _database = new SQLiteAsyncConnection(dbPath);

        // Créer les tables si elles n'existent pas
        await _database.CreateTableAsync<Models.Local.LocalBooking>();
        await _database.CreateTableAsync<Models.Local.LocalRide>();
        await _database.CreateTableAsync<Models.Local.LocalCity>();
        await _database.CreateTableAsync<LocalPublishedRide>();
        await _database.CreateTableAsync<LocalDriverRequest>();
    }

    // --- OPÉRATIONS RÉSERVATIONS ---

    public async Task SaveBookingsAsync(List<Models.Local.LocalBooking> bookings)
    {
        await Init();
        // On vide l'ancien cache et on remplace par le nouveau (stratégie simple)
        await _database.DeleteAllAsync<Models.Local.LocalBooking>();
        await _database.InsertAllAsync(bookings);
    }

    public async Task<List<Models.Local.LocalBooking>> GetBookingsAsync()
    {
        await Init();
        return await _database.Table<Models.Local.LocalBooking>().ToListAsync();
    }



    // Dans DatabaseService.cs

    // N'oublie pas d'ajouter la table dans Init() :
    // await _database.CreateTableAsync<Models.Local.LocalRide>();

    public async Task SaveRidesCacheAsync(List<Models.Local.LocalRide> rides)
    {
        await Init();
        // On garde seulement les trajets récents en local
        await _database.DeleteAllAsync<Models.Local.LocalRide>();
        await _database.InsertAllAsync(rides);
    }

    public async Task<List<Models.Local.LocalRide>> GetCachedRidesAsync()
    {
        await Init();
        return await _database.Table<Models.Local.LocalRide>()
                              .OrderByDescending(r => r.DepartureTime)
                              .ToListAsync();
    }

    // Recherche locale dans SQLite (Mode Hors-ligne)
    public async Task<List<Models.Local.LocalRide>> SearchRidesLocalAsync(string from, string to)
    {
        // 1. Toujours s'assurer que la connexion est ouverte
        await Init();

        // 2. Faire la requête SQL via LINQ
        // On utilise ToLower() pour que la recherche marche même avec des majuscules/minuscules
        var results = await _database.Table<Models.Local.LocalRide>()
            .Where(r => r.StartLocation.ToLower() == from.ToLower() &&
                        r.EndLocation.ToLower() == to.ToLower())
            .OrderByDescending(r => r.DepartureTime)
            .ToListAsync();

        return results;
    }
    // Dans ton DatabaseService.cs

    public async Task SyncCitiesFromCloudAsync(List<string> cloudCities)
    {
        await Init();

        if (cloudCities == null || !cloudCities.Any()) return;

        // 1. On transforme la liste de string en objets SQLite
        var localCities = cloudCities.Select(c => new Models.Local.LocalCity { Name = c }).ToList();

        // 2. On vide l'ancienne liste
        await _database.DeleteAllAsync<Models.Local.LocalCity>();

        // 3. On insère la nouvelle
        await _database.InsertAllAsync(localCities);

        Console.WriteLine("✅ Villes synchronisées avec succès depuis le Cloud !");
    }
    public async Task SaveCitiesAsync(List<Models.Local.LocalCity> cities)
    {
        await Init();
        await _database.DeleteAllAsync<Models.Local.LocalCity>();
        await _database.InsertAllAsync(cities);
    }

    public async Task<List<LocalCity>> GetCitiesAsync()
    {
        try
        {
            await Init(); // Ouvre la connexion
            var list = await _database.Table<LocalCity>().ToListAsync();
            return list ?? new List<LocalCity>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur SQLite GetCities: {ex.Message}");
            return new List<LocalCity>();
        }
    }
    // --- GESTION DES TRAJETS PUBLIÉS ---
    public async Task SaveMyRidesAsync(List<LocalPublishedRide> rides)
    {
        await Init();
        await _database.DeleteAllAsync<LocalPublishedRide>();
        await _database.InsertAllAsync(rides);
    }

    public async Task<List<LocalPublishedRide>> GetMyRidesAsync()
    {
        await Init();
        return await _database.Table<LocalPublishedRide>().ToListAsync();
    }

    // --- GESTION DES DEMANDES PASSAGERS ---
    public async Task SaveDriverRequestsAsync(List<LocalDriverRequest> requests)
    {
        await Init();
        await _database.DeleteAllAsync<LocalDriverRequest>();
        await _database.InsertAllAsync(requests);
    }

    public async Task<List<LocalDriverRequest>> GetDriverRequestsAsync()
    {
        await Init();
        return await _database.Table<LocalDriverRequest>().ToListAsync();
    }
    // Supprime une demande spécifique du cache (quand elle est traitée)
    public async Task DeleteLocalRequestAsync(int bookingId)
    {
        try
        {
            // 1. On s'assure que la connexion à la base est ouverte
            await Init();

            // 2. On supprime l'entrée par son ID (Clé primaire)
            // On précise bien le type <LocalDriverRequest>
            await _database.DeleteAsync<Models.Local.LocalDriverRequest>(bookingId);

            Console.WriteLine($"[SQLITE] Demande #{bookingId} supprimée du cache local.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SQLITE ERROR] Échec de suppression de la demande: {ex.Message}");
        }
    }
}