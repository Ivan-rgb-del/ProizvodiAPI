using MediatR;
using ProizvodiAPI.Application.DTOs;
using ProizvodiAPI.Application.Interfaces;
using ProizvodiAPI.Application.Mapping;
using ProizvodiAPI.Domain.Models;

namespace ProizvodiAPI.Application.Proizvodi.Command
{
    public record KreirajProizovdCommand(KreirajProizvodDTO podaci) : IRequest<ProizvodResponseDTO>;

    public class KreirajProizvodCommandHandler
        : IRequestHandler<KreirajProizovdCommand, ProizvodResponseDTO>
    {
        private readonly IProizvodRepository _repository;
        public ILogger<KreirajProizvodCommandHandler> _logger { get; }

        public KreirajProizvodCommandHandler(
            IProizvodRepository repository,
            ILogger<KreirajProizvodCommandHandler> logger
        )
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<ProizvodResponseDTO> Handle(KreirajProizovdCommand request, CancellationToken cancellationToken)
        {
            var dto = request.podaci;

            var proizvod = new Proizvod
            {
                Naziv = dto.Naziv,
                Opis = dto.Opis,
                Cena = dto.Cena,
                Kolicina = dto.Kolicina,
                Kategorija = dto.Kategorija,
                KreiranDatum = DateTime.Now
            };

            var kreiranProizvod = await _repository.Kreiraj(proizvod);
            _logger.LogInformation("Kreiran proizvod {Naziv} sa Id {Id}", kreiranProizvod.Naziv, kreiranProizvod.Id);
            return kreiranProizvod.ToResponseDTO();
        }
    }
}
