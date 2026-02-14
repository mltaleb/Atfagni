using Atfagni.API.Data;
using Atfagni.API.Endpoints;
using Microsoft.EntityFrameworkCore;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// --- DATABASE CONFIGURATION ---

var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

if (!string.IsNullOrEmpty(databaseUrl))
{
    // We are on Render (production)
    var uri = new Uri(databaseUrl);
    var userInfo = uri.UserInfo.Split(':');

    var connectionString = new NpgsqlConnectionStringBuilder
    {
        Host = uri.Host,
        Port = uri.Port > 0 ? uri.Port : 5432,
        Username = userInfo[0],
        Password = userInfo[1],
        Database = uri.AbsolutePath.Trim('/'),
        SslMode = SslMode.Require,
        TrustServerCertificate = true
    }.ToString();

    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(connectionString));
}
else
{
    // Local development
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(
            builder.Configuration.GetConnectionString("DefaultConnection")));
}


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
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.Run();
