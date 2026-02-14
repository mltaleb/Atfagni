using Atfagni.API.Data;
using Atfagni.API.Endpoints;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
// Ajoutez ces lignes pour dire à l'API d'utiliser PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}
//app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();

app.UseHttpsRedirection();
// --- MAPPAGE DES MODULES (La Méthode Pro) ---
app.MapUserEndpoints();
app.MapRideEndpoints();
app.MapBookingEndpoints();
// Demain vous ajouterez : app.MapBookingEndpoints();

app.Run();

