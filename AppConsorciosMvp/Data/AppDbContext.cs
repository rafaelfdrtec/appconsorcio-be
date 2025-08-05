using AppConsorciosMvp.Models;
using Microsoft.EntityFrameworkCore;

namespace AppConsorciosMvp.Data
{
    /// <summary>
    /// Contexto do banco de dados para a aplicação
    /// </summary>
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// DbSet de usuários
        /// </summary>
        public DbSet<Usuario> Usuarios { get; set; } = null!;

        /// <summary>
        /// DbSet de cartas de consórcio
        /// </summary>
        public DbSet<CartaConsorcio> CartasConsorcio { get; set; } = null!;

        /// <summary>
        /// Configura o modelo ao criar o banco de dados
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuração da entidade Usuário
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();

                // Um usuário pode ter várias cartas (como vendedor)
                entity.HasMany(u => u.CartasConsorcio)
                      .WithOne(c => c.Vendedor)
                      .HasForeignKey(c => c.VendedorId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuração da entidade CartaConsorcio
            modelBuilder.Entity<CartaConsorcio>(entity =>
            {
                // Índice para melhorar a performance de pesquisa por tipo de bem
                entity.HasIndex(e => e.TipoBem);

                // Índice para melhorar a performance de pesquisa por status
                entity.HasIndex(e => e.Status);
            });
        }
    }
}
