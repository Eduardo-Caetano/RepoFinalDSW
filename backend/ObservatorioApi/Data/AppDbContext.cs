using Microsoft.EntityFrameworkCore;
using ObservatorioApi.Models;

namespace ObservatorioApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Deputado> Deputados { get; set; }
    public DbSet<Despesa> Despesas { get; set; }
    public DbSet<PerfilAnalise> PerfisAnalise { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Deputado>()
            .HasIndex(deputado => deputado.IdCamara)
            .IsUnique();

        modelBuilder.Entity<Despesa>()
            .Property(despesa => despesa.Valor)
            .HasColumnType("decimal(10,2)");

        modelBuilder.Entity<Deputado>()
            .HasMany(deputado => deputado.Despesas)
            .WithOne(despesa => despesa.Deputado)
            .HasForeignKey(despesa => despesa.DeputadoId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Deputado>()
            .HasOne(deputado => deputado.PerfilAnalise)
            .WithOne(perfilAnalise => perfilAnalise.Deputado)
            .HasForeignKey<PerfilAnalise>(perfilAnalise => perfilAnalise.DeputadoId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PerfilAnalise>()
            .HasIndex(perfilAnalise => perfilAnalise.DeputadoId)
            .IsUnique();

        modelBuilder.Entity<PerfilAnalise>()
            .Property(perfilAnalise => perfilAnalise.Resumo)
            .HasColumnType("text");

        modelBuilder.Entity<PerfilAnalise>()
            .Property(perfilAnalise => perfilAnalise.Observacoes)
            .HasColumnType("text");
    }
}
