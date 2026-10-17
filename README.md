# ProizvodiAPI

REST API za upravljanje proizvodima, napravljen u ASP.NET Core 8. Podržava CRUD operacije, JWT autentikaciju sa ulogama i čuva podatke u SQL Server bazi. Projekat je rađen kao vežba Clean Architecture, CQRS i testiranja.

## Tehnologije

- ASP.NET Core 8 (Web API, Controllers)
- Entity Framework Core + SQL Server (LocalDB)
- JWT autentikacija (uloge Admin i Korisnik)
- MediatR (CQRS: Query i Command handleri)
- FluentValidation
- Serilog (logovi u konzolu i fajl)
- Swagger / OpenAPI
- xUnit + Moq (unit testovi)

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

### Preduslovi

- .NET 8 SDK
- SQL Server LocalDB (dolazi uz Visual Studio)

### Koraci

1. Kloniraj repozitorijum i uđi u folder projekta:
```
   git clone <URL-repozitorijuma>
   cd ProizvodiAPI/ProizvodiAPI
```

2. Podesi tajni JWT ključ (najmanje 32 karaktera). Ključ nije u repozitorijumu:
```
   dotnet user-secrets init
   dotnet user-secrets set "Jwt:Key" "tvoj-sopstveni-dugacak-tajni-kljuc-od-32+-karaktera"
```

3. Napravi bazu primenom migracija:
```
   dotnet ef database update
```
   (ili `Update-Database` u Package Manager Console u Visual Studiju)

4. Pokreni aplikaciju:
```
   dotnet run
```

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

```
dotnet test
```

## Logovi

Serilog upisuje logove u konzolu i u folder `Logs/` (jedan fajl po danu).