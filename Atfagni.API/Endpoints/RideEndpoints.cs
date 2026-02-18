using Atfagni.API.Data;
using Atfagni.API.Entities;
using Atfagni.Shared.DTOs;
using Atfagni.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace Atfagni.API.Endpoints;

public static class RideEndpoints
{
    public static void MapRideEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/rides").WithTags("Rides");

        // PUBLIER UN TRAJET
        group.MapPost("/", async (CreateRideRequest req, AppDbContext db) =>
        {
            // 1. Calcul de la Commission (Règle Métier)
            decimal commission = CalculateCommission(req.StartLocation, req.EndLocation);

            // 2. Création de l'entité
            var ride = new Ride
            {
                DriverId = req.DriverId,
                StartLocation = req.StartLocation,
                EndLocation = req.EndLocation,
                DepartureTime = req.DepartureTime.ToUniversalTime(), // Attention à l'heure UTC/Locale

                PricePerSeat = req.PricePerSeat,
                TotalSeats = req.TotalSeats,
                AvailableSeats = req.TotalSeats, // Au début, tout est libre

                AcceptsPassengers = req.AcceptsPassengers,
                AcceptsPackages = req.AcceptsPackages,
                PricePerPackage = req.PricePerPackage,
                PackageDescription = req.PackageDescription,

                Status = RideStatus.Scheduled,
                CommissionAmount = commission
            };

            db.Rides.Add(ride);
            await db.SaveChangesAsync();

            return Results.Ok(new { Message = "Trajet publié avec succès", RideId = ride.Id, Commission = commission });
        });

        // ... Dans MapRideEndpoints ...

        // 2. READ - "Mes Trajets" (Pour le chauffeur connecté)
        group.MapGet("/driver/{driverId}", async (int driverId, AppDbContext db) =>
        {
            var rides = await db.Rides
                .Where(r => r.DriverId == driverId)
                .OrderByDescending(r => r.DepartureTime) // Les plus récents en premier
                .Select(r => new RideDto
                {
                    Id = r.Id,
                    StartLocation = r.StartLocation,
                    EndLocation = r.EndLocation,
                    DepartureTime = r.DepartureTime,
                    Price = r.PricePerSeat,
                    AvailableSeats = r.AvailableSeats,
                    Status = r.Status
                })
                .ToListAsync();

            return Results.Ok(rides);
        });

        // 3. UPDATE - Modifier un trajet
        group.MapPut("/{id}", async (int id, UpdateRideRequest req, AppDbContext db) =>
        {
            var ride = await db.Rides.FindAsync(id);
            if (ride == null) return Results.NotFound();

            // Vérification de sécurité : Est-ce bien le bon chauffeur ?
            if (ride.DriverId != req.DriverId) return Results.Forbid();

            // Mise à jour des champs
            ride.StartLocation = req.StartLocation;
            ride.EndLocation = req.EndLocation;
            ride.DepartureTime = req.DepartureTime.ToUniversalTime(); // N'oubliez pas le UTC !
            ride.PricePerSeat = req.PricePerSeat;
            ride.TotalSeats = req.TotalSeats;

            // Si on change le nombre total de places, on ajuste les places dispos
            // (Logique simplifiée, attention s'il y a déjà des réservations)
            ride.AvailableSeats = req.TotalSeats;

            ride.AcceptsPassengers = req.AcceptsPassengers;
            ride.AcceptsPackages = req.AcceptsPackages;
            ride.PricePerPackage = req.PricePerPackage;
            ride.PackageDescription = req.PackageDescription;

            await db.SaveChangesAsync();
            return Results.Ok(new { Message = "Trajet mis à jour" });
        });

        // 4. DELETE - Supprimer un trajet
        group.MapDelete("/{id}", async (int id, AppDbContext db) =>
        {
            var ride = await db.Rides.FindAsync(id);
            if (ride == null) return Results.NotFound();

            // Option 1 : Suppression physique (On efface tout)
            // db.Rides.Remove(ride);

            // Option 2 (Recommandée) : Soft Delete (On marque comme Annulé)
            ride.Status = RideStatus.Cancelled;

            await db.SaveChangesAsync();
            return Results.Ok(new { Message = "Trajet annulé" });
        });
        // RECHERCHE DE TRAJETS (Pour Passagers)
        group.MapGet("/search", async (string? from, string? to, DateTime? date, AppDbContext db) =>
        {
            var query = db.Rides
                .Include(r => r.Driver)
                .Where(r => r.Status == RideStatus.Scheduled && r.AvailableSeats > 0);

            // Filtres dynamiques
            if (!string.IsNullOrWhiteSpace(from))
                query = query.Where(r => r.StartLocation.ToLower().Contains(from.ToLower()));

            if (!string.IsNullOrWhiteSpace(to))
                query = query.Where(r => r.EndLocation.ToLower().Contains(to.ToLower()));

            if (date.HasValue)
            {
                // On cherche les trajets du jour demandé (ignorer l'heure)
                var searchDate = date.Value.Date;
                query = query.Where(r => r.DepartureTime.Date == searchDate);
            }

            // On trie par date la plus proche
            var rides = await query.OrderBy(r => r.DepartureTime)
                .Select(r => new RideDto
                {
                    Id = r.Id,
                    DriverName = r.Driver.FullName,
                    StartLocation = r.StartLocation,
                    EndLocation = r.EndLocation,
                    DepartureTime = r.DepartureTime,
                    Price = r.PricePerSeat,
                    AvailableSeats = r.AvailableSeats,
                    // Info importante pour le passager qui veut envoyer un colis
                    AcceptsPackages = r.AcceptsPackages
                })
                .ToListAsync();

            return Results.Ok(rides);
        });
        // GET /api/cities
        group.MapGet("/cities", async (AppDbContext db) =>
    await db.Cities.OrderBy(c => c.Name).Select(c => c.Name).ToListAsync());
        // GET /api/rides/latest
        group.MapGet("/latest", async (AppDbContext db) =>
        {
            return await db.Rides
                .Include(r => r.Driver)
                //.Where(r => r.Status == RideStatus.Scheduled && r.AvailableSeats > 0)
                .OrderByDescending(r => r.Id) // Les plus récents en premier
                .Take(10) // On en prend 10 par défaut
                .Select(r => new RideDto
                {
                    Id = r.Id,
                    DriverName = r.Driver.FullName,
                    StartLocation = r.StartLocation,
                    EndLocation = r.EndLocation,
                    DepartureTime = r.DepartureTime,
                    Price = r.PricePerSeat,
                    AvailableSeats = r.AvailableSeats,
                    AcceptsPackages = r.AcceptsPackages
                })
                .ToListAsync();
        });
    }

    // FONCTION PRIVÉE POUR CALCULER LE PRIX DE TA COMMISSION
    private static decimal CalculateCommission(string start, string end)
    {
        start = start.ToLower();
        end = end.ToLower();

        // Liste des villes d'Europe (à enrichir ou mettre en DB plus tard)
        var europeCities = new[] { "paris", "madrid", "barcelone", "lyon", "marseille", "bordeaux", "alicante" };

        bool startInEurope = europeCities.Any(c => start.Contains(c));
        bool endInEurope = europeCities.Any(c => end.Contains(c));

        if (startInEurope || endInEurope)
        {
            return 5.0m; // 5€ si ça touche l'Europe (ou l'équivalent en devise locale)
        }
        else
        {
            // Trajet local (Camps, Tindouf, Zouerate, Nouadhibou...)
            return 100.0m; // 100 DA (ou montant local faible)
        }

    }



}