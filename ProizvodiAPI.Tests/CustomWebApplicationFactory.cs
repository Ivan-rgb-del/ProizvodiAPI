using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProizvodiAPI.Infrastructure.Data;

namespace ProizvodiAPI.Tests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        // SQLite in-memory živi dok je konekcija otvorena
        private readonly SqliteConnection _connection = new("DataSource=:memory:");

        public CustomWebApplicationFactory()
        {
            // CI nema User Secrets, a placeholder iz appsettings.json je prekratak
            Environment.SetEnvironmentVariable("Jwt__Key",
                "TestKljucKojiImaViseOdTridesetDvaKaraktera123!");
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");
            _connection.Open();

            builder.ConfigureServices(services =>
            {
                // Ukloni SQL Server konfiguraciju
                var opcije = services.SingleOrDefault(
                    s => s.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (opcije != null) services.Remove(opcije);

                services.AddDbContext<AppDbContext>(o => o.UseSqlite(_connection));

                // Napravi šemu iz modela
                using var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                scope.ServiceProvider.GetRequiredService<AppDbContext>()
                    .Database.EnsureCreated();
            });
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing) _connection.Dispose();
        }
    }
}
