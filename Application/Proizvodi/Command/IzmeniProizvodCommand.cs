using MediatR;
using ProizvodiAPI.Application.DTOs;
using ProizvodiAPI.Application.Interfaces;
using ProizvodiAPI.Application.Mapping;

namespace ProizvodiAPI.Application.Proizvodi.Command
{
    public record IzmeniProizvodCommand(int id, AzurirajProizvodDTO podaci) : IRequest<ProizvodResponseDTO?>;

    public class IzmeniProizvodCommandHandler
        : IRequestHandler<IzmeniProizvodCommand, ProizvodResponseDTO?>
    {
        private readonly IProizvodRepository _repository;

        public IzmeniProizvodCommandHandler(IProizvodRepository repository)
        {
            _repository = repository;
        }

        public async Task<ProizvodResponseDTO> Handle(IzmeniProizvodCommand request, CancellationToken cancellationToken)
        {
            var proizvod = await _repository.DajPoId(request.id);
            if (proizvod == null) return null;

            var dto = request.podaci;
            proizvod.Naziv = dto.Naziv;
            proizvod.Opis = dto.Opis;
            proizvod.Cena = dto.Cena;
            proizvod.Kolicina = dto.Kolicina;
            proizvod.Kategorija = dto.Kategorija;

            var izmenjeniProizvod = await _repository.Izmeni(proizvod);
            return izmenjeniProizvod.ToResponseDTO();
        }
    }
}
