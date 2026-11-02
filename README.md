# ProizvodiAPI

![CI](https://github.com/Ivan-rgb-del/ProizvodiAPI/actions/workflows/ci.yml/badge.svg)

REST API za upravljanje proizvodima, napravljen u ASP.NET Core 8. Podržava CRUD operacije, JWT autentikaciju sa ulogama i čuva podatke u SQL Server bazi. Projekat je rađen kao vežba Clean Architecture, CQRS i testiranja.

## Tehnologije

- ASP.NET Core 8 (Web API, Controllers)
- Entity Framework Core + SQL Server (LocalDB lokalno, kontejner u Dockeru)
- JWT autentikacija (uloge Admin i Korisnik)
- MediatR (CQRS: Query i Command handleri)
- FluentValidation
- Serilog (logovi u konzolu i fajl)
- Swagger / OpenAPI
- xUnit + Moq (unit testovi), WebApplicationFactory (integration testovi)
- Docker i Docker Compose
- GitHub Actions (CI)

## Arhitektura

Clean Architecture kroz foldere u jednom projektu. Zavisnosti idu ka unutra, ka Domain sloju.

```
ProizvodiAPI/
├── Api/             Controllers, Middleware
├── Application/     DTOs, Interfaces, Validators, Mappings, Query/Command handleri
├── Domain/          Modeli (Proizvod)
├── Infrastructure/  AppDbContext, Repositories, Auth
└── Migrations/      EF Core migracije
```

## Pokretanje

```
git clone https://github.com/Ivan-rgb-del/ProizvodiAPI.git
cd ProizvodiAPI
```

### Opcija 1: Docker (preporučeno)

Potreban je samo Docker Desktop.

1. Kopiraj `.env.example` u `.env` i upiši svoje vrednosti
   (`JWT_KEY` mora imati najmanje 32 karaktera, a `DB_PASSWORD` mora biti jaka lozinka za SQL Server):
   ```
   copy .env.example .env
   ```
2. Pokreni API i bazu:
   ```
   docker compose up --build
   ```
3. Swagger: http://localhost:8080/swagger

Migracije se primenjuju automatski pri startu, a podaci se čuvaju u Docker volume-u `sqldata`.
Zaustavljanje: `docker compose down` (dodaj `-v` ako želiš da obrišeš i bazu).

### Opcija 2: lokalno (.NET 8 SDK + LocalDB)

Preduslovi: .NET 8 SDK i SQL Server LocalDB (dolazi uz Visual Studio).
Za migracije je potreban alat `dotnet-ef`: `dotnet tool install --global dotnet-ef`.

1. Uđi u folder projekta:
   ```
   cd ProizvodiAPI
   ```
2. Podesi tajni JWT ključ (najmanje 32 karaktera, ključ nije u repozitorijumu):
   ```
   dotnet user-secrets init
   dotnet user-secrets set "Jwt:Key" "tvoj-kljuc-od-najmanje-32-karaktera"
   ```
3. Napravi bazu primenom migracija:
   ```
   dotnet ef database update
   ```
4. Pokreni aplikaciju:
   ```
   dotnet run
   ```
5. Swagger je na adresi koju ispiše `dotnet run` (nastavak `/swagger`).

## Autentikacija

Test korisnici (samo za razvoj):

| Korisničko ime | Lozinka | Uloga |
|---|---|---|
| admin | admin123 | Admin |
| korisnik | korisnik123 | Korisnik |

1. Pozovi `POST /api/Auth/login` i kopiraj `token` iz odgovora.
2. U Swagger-u klikni **Authorize** i unesi `Bearer {token}`.

## Endpoint-i

| Metoda | Ruta | Opis | Pristup |
|---|---|---|---|
| POST | `/api/Auth/login` | Prijava, vraća JWT token | Javno |
| GET | `/api/Proizvod` | Lista svih proizvoda | Prijavljen korisnik |
| GET | `/api/Proizvod/{id}` | Proizvod po ID-u | Prijavljen korisnik |
| POST | `/api/Proizvod` | Kreiranje proizvoda | Prijavljen korisnik |
| PUT | `/api/Proizvod/{id}` | Ažuriranje proizvoda | Prijavljen korisnik |
| DELETE | `/api/Proizvod/{id}` | Brisanje proizvoda | Samo Admin |

Poslovno pravilo: proizvod koji je na stanju (količina veća od 0) ne može se obrisati (`409 Conflict`).

## Testovi

- Unit testovi (xUnit i Moq): handleri izolovano
- Integration testovi (`WebApplicationFactory` i SQLite in-memory): HTTP zahtevi kroz celu aplikaciju,
  uključujući `401`, `403`, `409`, `400` i `204`

```
dotnet test
```

## CI

GitHub Actions pri svakom push-u i pull request-u radi restore, build, testove i Docker build.

## Logovi

Serilog upisuje logove u konzolu i u folder `Logs/` (jedan fajl po danu).