using Moq;
using ProizvodiAPI.Application.Interfaces;
using ProizvodiAPI.Application.Proizvodi.Command;
using ProizvodiAPI.Domain.Models;

namespace ProizvodiAPI.Tests
{
    public class ObrisiProizvodCommandHandlerTest
    {
        [Fact]
        public async Task Handle_ProizvodNaStanju_BacaInvalidOperationException()
        {
            // Arrange - pripremi lažni repository
            var repo = new Mock<IProizvodRepository>();
            repo.Setup(r => r.DajPoId(1))
                .ReturnsAsync(new Proizvod { Id = 1, Kolicina = 5 });

            var handler = new ObrisiProizvodCommandHandler(repo.Object);

            // Act + Assert - očekujemo grešku
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                handler.Handle(new ObrisiProizvodCommand(1), CancellationToken.None));

            // Repository.Obrisi se nikad nije smeo pozvati
            repo.Verify(r => r.Obrisi(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ProizvodNePostoji_VracaFalse()
        {
            var repo = new Mock<IProizvodRepository>();
            repo.Setup(r => r.DajPoId(99)).ReturnsAsync((Proizvod?)null);

            var handler = new ObrisiProizvodCommandHandler(repo.Object);

            var rezultat = await handler.Handle(new ObrisiProizvodCommand(99), CancellationToken.None);

            Assert.False(rezultat);
            repo.Verify(r => r.Obrisi(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ProizvodBezZaliha_BrisePoizvod()
        {
            var repo = new Mock<IProizvodRepository>();
            repo.Setup(r => r.DajPoId(1)).ReturnsAsync(new Proizvod { Id = 1, Kolicina = 0 });
            repo.Setup(r => r.Obrisi(1)).ReturnsAsync(true);

            var handler = new ObrisiProizvodCommandHandler(repo.Object);

            var rezultat = await handler.Handle(new ObrisiProizvodCommand(1), CancellationToken.None);

            Assert.True(rezultat);
            repo.Verify(r => r.Obrisi(1), Times.Once);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(50)]
        [InlineData(1000)]
        public async Task Handle_RazneKolicineNaStanju_UvekBacaGresku(int kolicina)
        {
            var repo = new Mock<IProizvodRepository>();
            repo.Setup(r => r.DajPoId(1)).ReturnsAsync(new Proizvod { Id = 1, Kolicina = kolicina });

            var handler = new ObrisiProizvodCommandHandler(repo.Object);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                handler.Handle(new ObrisiProizvodCommand(1), CancellationToken.None));
        }
    }
}
