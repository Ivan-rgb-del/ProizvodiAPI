using Microsoft.EntityFrameworkCore;
using ProizvodiAPI.Application.Interfaces;
using ProizvodiAPI.Domain.Models;
using ProizvodiAPI.Infrastructure.Data;

namespace ProizvodiAPI.Infrastructure.Repositories
{
    public class ProizvodiRepository : IProizvodRepository
    {
        private readonly AppDbContext _dbContext;

        public ProizvodiRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Proizvod?> DajPoId(int id)
        {
            return await _dbContext.Proizvodi.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Proizvod>> DajSve()
        {
            return await _dbContext.Proizvodi.ToListAsync();
        }

        public async Task<Proizvod?> Izmeni(Proizvod proizvod)
        {
            _dbContext.Proizvodi.Update(proizvod);

            await _dbContext.SaveChangesAsync(); ;

            return proizvod;
        }

        public async Task<Proizvod> Kreiraj(Proizvod proizvod)
        {
            _dbContext.Proizvodi.Add(proizvod);

            await _dbContext.SaveChangesAsync();

            return proizvod;
        }

        public async Task<bool> Obrisi(int id)
        {
            var proizvod = await DajPoId(id);

            if (proizvod == null) return false;

            _dbContext.Proizvodi.Remove(proizvod);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> AzurirajViseProizvoda(List<Proizvod> proizvodi)
        {
            using var transakcija = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                foreach (var item in proizvodi)
                {
                    _dbContext.Update(item);
                }

                await _dbContext.SaveChangesAsync();
                await transakcija.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                await transakcija.RollbackAsync();
                throw;
            }
        }
    }
}
