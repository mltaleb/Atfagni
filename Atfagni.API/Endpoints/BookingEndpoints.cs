using Atfagni.API.Data;
using Atfagni.API.Entities;
using Atfagni.Shared.DTOs;
using Atfagni.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace Atfagni.API.Endpoints;

public static class BookingEndpoints
{
    public static void MapBookingEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/bookings").WithTags("Bookings");

        // 1. RÉSERVER
        group.MapPost("/reserve", async (CreateBookingRequest req, AppDbContext db) =>
        {
            var ride = await db.Rides.FindAsync(req.RideId);
            if (ride == null) return Results.NotFound("Trajet introuvable.");

            if (ride.AvailableSeats < req.Seats)
                return Results.BadRequest("Pas assez de places disponibles.");

            var code = new Random().Next(1000, 9999).ToString();

            var booking = new Booking
            {
                RideId = req.RideId,
                PassengerId = req.PassengerId,
                SeatsBooked = req.Seats,
                Type = req.Type,
                PackageDetails = req.PackageDescription, // Mappe le DTO vers l'Entité
                ValidationCode = code,
                Status = BookingStatus.Pending,
                BookedAt = DateTime.UtcNow
            };

            db.Bookings.Add(booking);
            await db.SaveChangesAsync();

            return Results.Ok(new { Message = "Demande envoyée", BookingId = booking.Id });
        });

        // 2. LISTE DES DEMANDES (Le mapping crucial)
        group.MapGet("/driver/{driverId}/pending", async (int driverId, AppDbContext db) =>
        {
            var requests = await db.Bookings
                .Include(b => b.Passenger)
                .Include(b => b.Ride)
                .Where(b => b.Ride.DriverId == driverId && b.Status == BookingStatus.Pending)
                .Select(b => new BookingRequestDto // On utilise le DTO du Shared
                {
                    Id = b.Id,
                    PassengerName = b.Passenger.FullName,
                    PassengerPhone = b.Passenger.PhoneNumber,
                    Type = b.Type.ToString(),

                    // Mappe l'Entité vers le DTO
                    PackageDescription = b.PackageDetails,

                    SeatsRequested = b.SeatsBooked,
                    TripDescription = $"{b.Ride.StartLocation} -> {b.Ride.EndLocation}"
                })
                .ToListAsync();

            return Results.Ok(requests);
        });

        // 3. DECISION (Accepter/Refuser)
        group.MapPost("/{bookingId}/decide", async (int bookingId, bool accept, AppDbContext db) =>
        {
            var booking = await db.Bookings.Include(b => b.Ride).FirstOrDefaultAsync(b => b.Id == bookingId);
            if (booking == null) return Results.NotFound();

            if (accept)
            {
                if (booking.Ride.AvailableSeats < booking.SeatsBooked)
                    return Results.BadRequest("Plus de place.");

                booking.Status = BookingStatus.Accepted;
                booking.Ride.AvailableSeats -= booking.SeatsBooked;
            }
            else
            {
                booking.Status = BookingStatus.Rejected;
            }

            await db.SaveChangesAsync();
            return Results.Ok(new { Status = booking.Status.ToString() });
        });
    }
}