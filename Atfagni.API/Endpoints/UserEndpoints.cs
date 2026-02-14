using Atfagni.API.Data;
using Atfagni.API.Entities;
using Atfagni.Shared.DTOs;
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
    }
}