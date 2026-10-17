using Microsoft.Extensions.Logging;
using Moq;
using ProizvodiAPI.Application.DTOs;
using ProizvodiAPI.Application.Interfaces;
using ProizvodiAPI.Application.Proizvodi.Command;
using ProizvodiAPI.Application.Proizvodi.Queries;
using ProizvodiAPI.Domain.Models;

namespace ProizvodiAPI.Tests
{
    public class KreirajProizvodCommandHandlerTests
    {
        [Fact]
        public async Task Handle_ValidniPodaci_SnimaProizvodSaPodacimaIzDTOa()
        {
            var repo = new Mock<IProizvodRepository>();
            var logger = new Mock<ILogger<KreirajProizvodCommandHandler>>();

            // Repository vraća isti objekat koji je primio, uz dodeljen Id
            repo.Setup(r => r.Kreiraj(It.IsAny<Proizvod>()))
                .ReturnsAsync((Proizvod p) => { p.Id = 7; return p; });

            var handler = new KreirajProizvodCommandHandler(repo.Object, logger.Object);
            var dto = new KreirajProizvodDTO
            {
                Naziv = "Tastatura",
                Opis = "Mehanička",
                Cena = 85,
                Kolicina = 20,
                Kategorija = "Elektronika"
            };

            var rezultat = await handler.Handle(new KreirajProizovdCommand(dto), CancellationToken.None);

            Assert.Equal(7, rezultat.Id);
            Assert.Equal("Tastatura", rezultat.Naziv);
            Assert.Equal(85, rezultat.Cena);

            // Proveravamo šta je tačno poslato repository-ju
            repo.Verify(r => r.Kreiraj(It.Is<Proizvod>(p =>
                p.Naziv == "Tastatura" && p.Kolicina == 20)), Times.Once);
        }


        [Fact]
        public async Task Handle_PostojeciId_VracaProizvod()
        {
            // Arrange
            var repo = new Mock<IProizvodRepository>();

            var proizvod = new Proizvod
            {
                Id = 7,
                Naziv = "Tastatura",
                Opis = "Mehanička",
                Cena = 85,
                Kolicina = 20,
                Kategorija = "Elektronika"
            };

            repo.Setup(r => r.DajPoId(7))
                .ReturnsAsync(proizvod);

            var handler = new DajProizvodPoIdQueryHandler(repo.Object);
            var query = new DajProizvodPoIdQuery(7);

            // Act
            var rezultat = await handler.Handle(
                query,
                CancellationToken.None);

            // Assert
            Assert.NotNull(rezultat);
            Assert.Equal(7, rezultat.Id);
            Assert.Equal("Tastatura", rezultat.Naziv);
            Assert.Equal(85, rezultat.Cena);

            repo.Verify(r => r.DajPoId(7), Times.Once);
        }

        [Fact]
        public async Task Handle_NepostojeciId_VracaNull()
        {
            // Arrange
            var repo = new Mock<IProizvodRepository>();

            repo.Setup(r => r.DajPoId(999))
                .ReturnsAsync((Proizvod?)null);

            var handler = new DajProizvodPoIdQueryHandler(repo.Object);
            var query = new DajProizvodPoIdQuery(999);

            // Act
            var rezultat = await handler.Handle(
                query,
                CancellationToken.None);

            // Assert
            Assert.Null(rezultat);

            repo.Verify(r => r.DajPoId(999), Times.Once);
        }
    }
}
