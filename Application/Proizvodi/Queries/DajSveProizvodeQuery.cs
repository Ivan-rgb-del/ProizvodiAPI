using MediatR;
using ProizvodiAPI.Application.DTOs;
using ProizvodiAPI.Application.Interfaces;
using ProizvodiAPI.Application.Mapping;

namespace ProizvodiAPI.Application.Proizvodi.Queries
{
    // Query = "pitanje" koje postavljamo. Ovde nema parametara. -> valjda ono sto ja zelim da dobijem nazad?
    public record DajSveProizvodeQuery : IRequest<List<ProizvodResponseDTO>>;

    // Handler = klasa koja odgovara na to pitanje
    public class DajSveProizvodeQueryHandler
        (IProizvodRepository _repository)
        : IRequestHandler<DajSveProizvodeQuery, List<ProizvodResponseDTO>>
    {
        //private readonly IProizvodRepository _repository;

        //public DajSveProizvodeQueryHandler(IProizvodRepository repository)
        //{
        //    _repository = repository;
        //}

        public async Task<List<ProizvodResponseDTO>> Handle(DajSveProizvodeQuery request, CancellationToken cancellationToken)
        {
            var proizvodi = await _repository.DajSve();
            return proizvodi.Select(p => p.ToResponseDTO()).ToList();
        }
    }
}
