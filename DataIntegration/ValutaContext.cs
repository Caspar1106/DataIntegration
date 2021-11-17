using Microsoft.EntityFrameworkCore;

namespace DataIntegration
{
    public interface IValutaContext
    {
        DbSet<ValutaConversion> conversions { get; }

        int SaveChanges();
    }

    public class ValutaContext : DbContext, IValutaContext
    {
        public DbSet<ValutaConversion> conversions => Set<ValutaConversion>();

        public ValutaContext(DbContextOptions<ValutaContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        modelBuilder
            .Entity<ValutaConversion>()
            .HasIndex(v => v.Id)
            .IsUnique();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseSqlServer("Server=52.157.108.214;Database=Caspar;User Id=Caspar;Password=TestCase;");
        }
    }
}