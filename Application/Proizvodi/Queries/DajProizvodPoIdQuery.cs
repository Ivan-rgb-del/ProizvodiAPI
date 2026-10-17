using MediatR;
using ProizvodiAPI.Application.DTOs;
using ProizvodiAPI.Application.Interfaces;
using ProizvodiAPI.Application.Mapping;

namespace ProizvodiAPI.Application.Proizvodi.Queries
{
    public record DajProizvodPoIdQuery(int Id) : IRequest<ProizvodResponseDTO?>;

    public class DajProizvodPoIdQueryHandler : IRequestHandler<DajProizvodPoIdQuery, ProizvodResponseDTO?>
    {
        private readonly IProizvodRepository _repository;

        public DajProizvodPoIdQueryHandler(IProizvodRepository repository)
        {
            _repository = repository;
        }

        public async Task<ProizvodResponseDTO?> Handle(DajProizvodPoIdQuery request, CancellationToken cancellationToken)
        {
            var proizvod = await _repository.DajPoId(request.Id);
            return proizvod?.ToResponseDTO();
        }
    }
}
