using ProizvodiAPI.Application.DTOs;
using ProizvodiAPI.Application.Interfaces;
using ProizvodiAPI.Domain.Models;

namespace ProizvodiAPI.Application.Services
{
    public class ProizvodService : IProizvodService
    {
        private readonly IProizvodRepository _proizovdRepository;

        public ProizvodService(IProizvodRepository proizovdInterface)
        {
            _proizovdRepository = proizovdInterface;
        }

        public async Task<List<ProizvodResponseDTO>> DajSveProizvode()
        {
            var proizvodi = await _proizovdRepository.DajSve();
            return proizvodi.Select(p => MapujUResponse(p)).ToList();
        }

        public async Task<ProizvodResponseDTO?> DajProizvodPoId(int id)
        {
            var proizvod = await _proizovdRepository.DajPoId(id);

            if (proizvod == null) return null;

            return MapujUResponse(proizvod);
        }

        public async Task<ProizvodResponseDTO> KreirajProizvod(KreirajProizvodDTO dto)
        {
            var proizvod = new Proizvod
            {
                Naziv = dto.Naziv,
                Opis = dto.Opis,
                Cena = dto.Cena,
                Kolicina = dto.Kolicina,
                Kategorija = dto.Kategorija,
                KreiranDatum = DateTime.Now
            };

            var kreiran = await _proizovdRepository.Kreiraj(proizvod);

            return MapujUResponse(kreiran);
        }

        public async Task<ProizvodResponseDTO?> AzurirajProizvod(int id, AzurirajProizvodDTO dto)
        {
            var proizvod = await _proizovdRepository.DajPoId(id);
            if (proizvod == null) return null;

            proizvod.Naziv = dto.Naziv;
            proizvod.Opis = dto.Opis;
            proizvod.Cena = dto.Cena;
            proizvod.Kolicina = dto.Kolicina;
            proizvod.Kategorija = dto.Kategorija;

            var azuriran = await _proizovdRepository.Izmeni(proizvod);
            return azuriran == null ? null : MapujUResponse(azuriran);
        }

        public async Task<bool> ObrisiProizvod(int id)
        {
            var proizvod = await _proizovdRepository.DajPoId(id);
            if (proizvod == null) return false;

            if (proizvod.Kolicina > 0)
                throw new InvalidOperationException("Ne možeš obrisati proizvod koji je na stanju! Prvo rasprodaj zalihe.");

            return await _proizovdRepository.Obrisi(id);
        }

        private ProizvodResponseDTO MapujUResponse(Proizvod p)
        {
            return new ProizvodResponseDTO
            {
                Id = p.Id,
                Naziv = p.Naziv,
                Opis = p.Opis,
                Cena = p.Cena,
                Kolicina = p.Kolicina,
                Kategorija = p.Kategorija,
                KreiranDatum = p.KreiranDatum
            };
        }
    }
}
