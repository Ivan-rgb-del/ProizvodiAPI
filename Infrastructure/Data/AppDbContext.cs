using Microsoft.EntityFrameworkCore;
using ProizvodiAPI.Domain.Models;

namespace ProizvodiAPI.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Proizvod> Proizvodi { get; set; }
    }
}
