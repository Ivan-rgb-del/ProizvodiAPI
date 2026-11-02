using ProizvodiAPI.Application.DTOs;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using static ProizvodiAPI.Application.DTOs.AuthDTO;

namespace ProizvodiAPI.Tests
{
    public class ProizvodiApiTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;

        public ProizvodiApiTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        private async Task<HttpClient> KlijentSaTokenom(string korisnik, string lozinka)
        {
            var client = _factory.CreateClient();
            var odgovor = await client.PostAsJsonAsync("/api/Auth/login",
                new LoginDTO { KorisnickoIme = korisnik, Lozinka = lozinka });
            odgovor.EnsureSuccessStatusCode();

            var token = await odgovor.Content.ReadFromJsonAsync<TokenResponseDTO>();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token!.Token);
            return client;
        }

        [Fact]
        public async Task GetProizvodi_BezTokena_Vraca401()
        {
            var client = _factory.CreateClient();
            var odgovor = await client.GetAsync("/api/Proizvod");
            Assert.Equal(HttpStatusCode.Unauthorized, odgovor.StatusCode);
        }

        [Fact]
        public async Task KreirajProizvod_ZatimDajSve_VracaKreiran()
        {
            var client = await KlijentSaTokenom("admin", "admin123");

            var kreiranje = await client.PostAsJsonAsync("/api/Proizvod", new KreirajProizvodDTO
            {
                Naziv = "Tastatura",
                Opis = "Mehanička",
                Cena = 85,
                Kolicina = 20,
                Kategorija = "Elektronika"
            });
            Assert.Equal(HttpStatusCode.Created, kreiranje.StatusCode);

            var lista = await client.GetFromJsonAsync<List<ProizvodResponseDTO>>("/api/Proizvod");
            Assert.Contains(lista!, p => p.Naziv == "Tastatura");
        }

        [Fact]
        public async Task Delete_ObicanKorisnik_Vraca403()
        {
            var client = await KlijentSaTokenom("korisnik", "korisnik123");
            var odgovor = await client.DeleteAsync("/api/Proizvod/1");
            Assert.Equal(HttpStatusCode.Forbidden, odgovor.StatusCode);
        }

        [Fact]
        public async Task Delete_ProizvodNaStanju_Vraca409()
        {
            var client = await KlijentSaTokenom("admin", "admin123");

            var kreiranje = await client.PostAsJsonAsync("/api/Proizvod", new KreirajProizvodDTO
            {
                Naziv = "Monitor",
                Opis = "Gaming monitor",
                Cena = 250,
                Kolicina = 5,
                Kategorija = "Elektronika"
            });

            Assert.Equal(HttpStatusCode.Created, kreiranje.StatusCode);

            var proizvod = await kreiranje.Content.ReadFromJsonAsync<ProizvodResponseDTO>();

            var brisanje = await client.DeleteAsync($"/api/Proizvod/{proizvod!.Id}");

            Assert.Equal(HttpStatusCode.Conflict, brisanje.StatusCode);
        }

        [Fact]
        public async Task KreirajProizvod_NevalidniPodaci_Vraca400()
        {
            var client = await KlijentSaTokenom("admin", "admin123");

            var odgovor = await client.PostAsJsonAsync("/api/Proizvod", new KreirajProizvodDTO
            {
                Naziv = "",
                Opis = "Nevalidan proizvod",
                Cena = -100,
                Kolicina = 10,
                Kategorija = "Elektronika"
            });

            Assert.Equal(HttpStatusCode.BadRequest, odgovor.StatusCode);
        }

        [Fact]
        public async Task Delete_ProizvodBezZaliha_Vraca204()
        {
            var client = await KlijentSaTokenom("admin", "admin123");

            var kreiranje = await client.PostAsJsonAsync("/api/Proizvod", new KreirajProizvodDTO
            {
                Naziv = "Stara tastatura",
                Opis = "Tastatura bez zaliha",
                Cena = 50,
                Kolicina = 0,
                Kategorija = "Elektronika"
            });

            Assert.Equal(HttpStatusCode.Created, kreiranje.StatusCode);

            var proizvod = await kreiranje.Content.ReadFromJsonAsync<ProizvodResponseDTO>();

            var brisanje = await client.DeleteAsync($"/api/Proizvod/{proizvod!.Id}");

            Assert.Equal(HttpStatusCode.NoContent, brisanje.StatusCode);
        }
    }
}
