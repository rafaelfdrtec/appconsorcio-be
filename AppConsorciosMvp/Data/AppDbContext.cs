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
        /// DbSet de administradoras
        /// </summary>
        public DbSet<Administradora> Administradoras { get; set; } = null!;

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

                // Definir tipos de coluna específicos para PostgreSQL
                entity.Property(e => e.Nome).HasColumnType("varchar(100)");
                entity.Property(e => e.Email).HasColumnType("varchar(100)");
                entity.Property(e => e.SenhaHash).HasColumnType("text");
                entity.Property(e => e.Papel).HasColumnType("varchar(20)");
            });

            // Configuração da entidade Administradora
            modelBuilder.Entity<Administradora>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Cnpj).IsUnique();
                entity.Property(e => e.Nome).IsRequired().HasColumnType("varchar(200)");
                entity.Property(e => e.Cnpj).IsRequired().HasColumnType("varchar(18)");
                entity.Property(e => e.Telefone).HasColumnType("varchar(20)");
                entity.Property(e => e.Email).HasColumnType("varchar(200)");
                entity.Property(e => e.Status).IsRequired().HasColumnType("varchar(10)").HasDefaultValue("ativa");
            });

            // Configuração da entidade CartaConsorcio
            modelBuilder.Entity<CartaConsorcio>(entity =>
            {
                // Índice para melhorar a performance de pesquisa por tipo de bem
                entity.HasIndex(e => e.TipoBem);

                // Índice para melhorar a performance de pesquisa por status
                entity.HasIndex(e => e.Status);

                // Índice para melhorar a performance de pesquisa por administradora
                entity.HasIndex(e => e.AdministradoraId);

                // Definir tipos de coluna específicos para PostgreSQL
                entity.Property(e => e.TipoBem).HasColumnType("varchar(50)");
                entity.Property(e => e.Status).HasColumnType("varchar(20)");
                entity.Property(e => e.Descricao).HasColumnType("text");
                entity.Property(e => e.ValorCredito).HasColumnType("decimal(18,2)");
                entity.Property(e => e.ValorEntrada).HasColumnType("decimal(18,2)");
                entity.Property(e => e.ValorParcela).HasColumnType("decimal(18,2)");

                // Configurar relacionamento com Administradora
                entity.HasOne(c => c.Administradora)
                    .WithMany(a => a.CartasConsorcio)
                    .HasForeignKey(c => c.AdministradoraId)
                    .OnDelete(DeleteBehavior.Restrict); // Não permite deletar administradora se tem cartas
            });


   
           
        }
    }
}

