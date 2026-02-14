# 1. On utilise l'image SDK pour CONSTRUIRE l'app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copier les fichiers projets (API + Shared)
COPY ["Atfagni.API/Atfagni.API.csproj", "Atfagni.API/"]
COPY ["Atfagni.Shared/Atfagni.Shared.csproj", "Atfagni.Shared/"]

# Restaurer les paquets NuGet
RUN dotnet restore "Atfagni.API/Atfagni.API.csproj"

# Copier tout le reste du code
COPY . .

# Construire l'application
WORKDIR "/src/Atfagni.API"
RUN dotnet build "Atfagni.API.csproj" -c Release -o /app/build

# Publier l'application (créer les DLL finaux)
FROM build AS publish
RUN dotnet publish "Atfagni.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 2. On utilise l'image légère ASP.NET pour EXÉCUTER l'app
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Render configure le port automatiquement via la variable PORT
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Atfagni.API.dll"]