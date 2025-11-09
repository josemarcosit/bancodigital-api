using BancoDigital.Api.Domain.Abstracoes;
using BancoDigital.Api.Domain.Entidades;
using BancoDigital.Api.Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;

namespace BancoDigital.Api.Infrastructure
{
    public class BancoDigitalDbContext : DbContext
    {
        public BancoDigitalDbContext(DbContextOptions<BancoDigitalDbContext> options)
            : base(options)
        { }

        public DbSet<Conta> Contas { get; set; } = null!;
        public DbSet<OutboxMessage> OutboxMessages { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=banco_digital.db")
                          .EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Ignore<EventoDomain>();

            modelBuilder.Entity<Conta>(b =>
            {
                b.HasKey(c => c.Id);
                b.Property(c => c.Saldo).IsRequired();
            });

            modelBuilder.Entity<OutboxMessage>(b =>
            {
                b.HasKey(o => o.Id);
                b.Property(o => o.Type).IsRequired();
                b.Property(o => o.Payload).IsRequired();
            });

        }
    }
}
