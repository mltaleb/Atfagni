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
    public async Task SaveCitiesAsync(List<Models.Local.LocalCity> cities)
    {
        await Init();
        await _database.DeleteAllAsync<Models.Local.LocalCity>();
        await _database.InsertAllAsync(cities);
    }

    public async Task<List<Models.Local.LocalCity>> GetCitiesAsync(string filter = "")
    {
        await Init();
        var query = _database.Table<Models.Local.LocalCity>();
        if (!string.IsNullOrWhiteSpace(filter))
            query = query.Where(c => c.Name.ToLower().Contains(filter.ToLower()));
        return await query.OrderBy(c => c.Name).ToListAsync();
    }
}