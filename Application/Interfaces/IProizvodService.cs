using ProizvodiAPI.Application.DTOs;

namespace ProizvodiAPI.Application.Interfaces
{
    public interface IProizvodService
    {
        Task<List<ProizvodResponseDTO>> DajSveProizvode();

        Task<ProizvodResponseDTO> DajProizvodPoId(int id);

        Task<ProizvodResponseDTO> KreirajProizvod(KreirajProizvodDTO dto);

        Task<ProizvodResponseDTO> AzurirajProizvod(int id, AzurirajProizvodDTO dto);

        Task<bool> ObrisiProizvod(int id);
    }
}
