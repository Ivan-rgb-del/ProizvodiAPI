# Faza 1: build (velika slika sa SDK-om)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Prvo samo .csproj, da se restore keširа dok se paketi ne menjaju
COPY ProizvodiAPI/ProizvodiAPI.csproj ProizvodiAPI/
RUN dotnet restore ProizvodiAPI/ProizvodiAPI.csproj

# Zatim ostatak koda
COPY ProizvodiAPI/ ProizvodiAPI/
RUN dotnet publish ProizvodiAPI/ProizvodiAPI.csproj -c Release -o /app/publish --no-restore

# Faza 2: runtime (mala slika, samo ono što treba za pokretanje)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "ProizvodiAPI.dll"]