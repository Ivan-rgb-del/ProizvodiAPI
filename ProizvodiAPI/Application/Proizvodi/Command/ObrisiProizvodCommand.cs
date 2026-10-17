using MediatR;
using ProizvodiAPI.Application.Interfaces;

namespace ProizvodiAPI.Application.Proizvodi.Command
{
    public record ObrisiProizvodCommand(int id) : IRequest<bool>;

    public class ObrisiProizvodCommandHandler
        : IRequestHandler<ObrisiProizvodCommand, bool>
    {
        private readonly IProizvodRepository _repository;

        public ObrisiProizvodCommandHandler(IProizvodRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(ObrisiProizvodCommand request, CancellationToken cancellationToken)
        {
            var proizvod = await _repository.DajPoId(request.id);
            if (proizvod == null) return false;

            // Poslovno pravilo živi ovde, ne u Repository-ju
            if (proizvod.Kolicina > 0)
            {
                throw new InvalidOperationException(
                    "Ne možeš obrisati proizvod koji je na stanju! Prvo rasprodaj zalihe."
                );
            }

            return await _repository.Obrisi(request.id);
        }
    }
}
