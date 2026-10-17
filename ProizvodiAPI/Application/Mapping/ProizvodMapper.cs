using ProizvodiAPI.Application.DTOs;
using ProizvodiAPI.Domain.Models;

namespace ProizvodiAPI.Application.Mapping
{
    public static class ProizvodMapper
    {
        public static ProizvodResponseDTO ToResponseDTO(this Proizvod p)
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
