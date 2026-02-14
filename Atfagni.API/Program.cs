using Atfagni.API.Data;
using Atfagni.API.Endpoints;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --- SERVICES ---

// 1. Ajouter la connexion PostgreSQL
// (Render remplacera la connexion locale par la variable d'environnement automatiquement)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Gestion des contrôleurs et JSON
builder.Services.AddControllers();

// 3. Configuration Swagger pour .NET 8 (L'ancienne méthode qui marche partout)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// --- MIDDLEWARE ---

// Activer Swagger même en production (pour que vous puissiez tester sur Render)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Atfagni API V1");
    c.RoutePrefix = "swagger"; // L'interface sera sur /swagger
});


app.UseHttpsRedirection();
app.UseAuthorization();

// --- ROUTES ---
app.MapControllers(); // Si vous utilisez des Controllers
// OU vos endpoints Minimal API :
app.MapUserEndpoints();
app.MapRideEndpoints();
app.MapBookingEndpoints();

app.Run();
