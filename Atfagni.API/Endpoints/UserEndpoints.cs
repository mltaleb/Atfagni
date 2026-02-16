using Atfagni.API.Data;
using Atfagni.API.Entities;
using Atfagni.Shared.DTOs;
using Atfagni.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace Atfagni.API.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users").WithTags("Users");

        // INSCRIPTION
        group.MapPost("/register", async (RegisterRequest req, AppDbContext db) =>
        {
            // 1. Vérifier si le téléphone existe déjà
            if (await db.Users.AnyAsync(u => u.PhoneNumber == req.PhoneNumber))
                return Results.BadRequest("Ce numéro est déjà utilisé.");

            // 2. Si c'est un chauffeur, vérifier les infos obligatoires
            if (req.Role == "Driver")
            {
                if (string.IsNullOrEmpty(req.CarModel) || req.DefaultSeats == null)
                    return Results.BadRequest("Les chauffeurs doivent préciser leur véhicule et le nombre de places.");
            }

            // 3. Créer l'utilisateur (Note: Normalement on "Hach" le mot de passe ici pour la sécurité)
            var newUser = new User
            {
                FullName = req.FullName,
                PhoneNumber = req.PhoneNumber,
                PasswordHash = req.Password, // À hacher en production !
                Role = req.Role,
                CarModel = req.CarModel,
                CarLicensePlate = req.CarLicensePlate,
                DefaultSeats = req.DefaultSeats,
                WalletBalance = 0
            };

            db.Users.Add(newUser);
            await db.SaveChangesAsync();

            return Results.Created($"/api/users/{newUser.Id}", new { newUser.Id, newUser.FullName, newUser.Role });
        });

        // CONNEXION (LOGIN)
        group.MapPost("/login", async (LoginRequest req, AppDbContext db) =>
        {
            var user = await db.Users
                .FirstOrDefaultAsync(u => u.PhoneNumber == req.PhoneNumber && u.PasswordHash == req.Password);

            if (user == null) return Results.Unauthorized();

            // On retourne l'objet qui correspond au UserDto du projet Shared
            return Results.Ok(new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Role = user.Role,
                PhoneNumber = user.PhoneNumber,
                WalletBalance = user.WalletBalance
            });
        });

        // Dans UserEndpoints ou DashboardEndpoints

        group.MapGet("/driver/{driverId}/dashboard", async (int driverId, AppDbContext db) =>
        {
            // 1. Récupérer le chauffeur
            var user = await db.Users.FindAsync(driverId);
            if (user == null) return Results.NotFound();

            // 2. Compter les demandes en attente (Pending)
            // On regarde les bookings liés aux trajets de ce chauffeur
            var pendingCount = await db.Bookings
                .Include(b => b.Ride)
                .CountAsync(b => b.Ride.DriverId == driverId && b.Status == BookingStatus.Pending);

            // 3. Trouver le prochain trajet (Futur et non annulé)
            var nextRide = await db.Rides
                .Where(r => r.DriverId == driverId && r.DepartureTime > DateTime.UtcNow && r.Status != RideStatus.Cancelled)
                .OrderBy(r => r.DepartureTime)
                .Select(r => new RideDto
                {
                    Id = r.Id,
                    StartLocation = r.StartLocation,
                    EndLocation = r.EndLocation,
                    DepartureTime = r.DepartureTime,
                    AvailableSeats = r.AvailableSeats,
                    Status = r.Status
                })
                .FirstOrDefaultAsync();

            // 4. Compter les trajets terminés
            var completedCount = await db.Rides.CountAsync(r => r.DriverId == driverId && r.DepartureTime < DateTime.UtcNow);

            return Results.Ok(new DriverDashboardDto
            {
                WalletBalance = user.WalletBalance,
                PendingRequestsCount = pendingCount,
                NextRide = nextRide,
                CompletedRidesCount = completedCount
            });
        });
    }
}