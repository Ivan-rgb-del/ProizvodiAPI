using ProizvodiAPI.Domain.Models;

namespace ProizvodiAPI.Application.Interfaces
{
    public interface IProizvodRepository
    {
        Task<List<Proizvod>> DajSve();

        Task<Proizvod?> DajPoId(int id);

        Task<Proizvod> Kreiraj(Proizvod proizvod);

        Task<Proizvod?> Izmeni(Proizvod proizvod);

        Task<bool> Obrisi(int id);
    }
}
