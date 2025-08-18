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

        // DbSets principais
        public DbSet<Usuario> Usuarios { get; set; } = null!;
        public DbSet<CartaConsorcio> CartasConsorcio { get; set; } = null!;
        public DbSet<Administradora> Administradoras { get; set; } = null!;
        public DbSet<DocumentoUsuario> DocumentosUsuario { get; set; } = null!;
        public DbSet<PropostaNegociacao> PropostasNegociacao { get; set; } = null!;

        // DbSets adicionais necessários
        public DbSet<Arquivo> Arquivos { get; set; } = null!;
        public DbSet<CartaAnexo> CartaAnexos { get; set; } = null!;
        public DbSet<PropostaAnexo> PropostaAnexos { get; set; } = null!;
        public DbSet<Anexo> Anexos { get; set; } = null!;
        public DbSet<ParametroSistema> ParametrosSistema { get; set; } = null!;
        // Alias para compatibilidade com serviços existentes
        public DbSet<ParametroSistema> Parametros { get; set; } = null!;

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
                      .OnDelete(DeleteBehavior.Restrict); // Não permite deletar usuário se tem cartas

                // Tipos de coluna
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
                // Índices para performance
                entity.HasIndex(e => e.TipoBem);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.AdministradoraId);
                entity.HasIndex(e => e.CompradorId);
                entity.HasIndex(e => e.PropostaVendaId);

                // Tipos de coluna
                entity.Property(e => e.TipoBem).HasColumnType("varchar(50)");
                entity.Property(e => e.Status).HasColumnType("varchar(20)");
                entity.Property(e => e.Descricao).HasColumnType("text");
                entity.Property(e => e.NumeroCota).IsRequired().HasColumnType("varchar(20)");
                entity.Property(e => e.Grupo).IsRequired().HasColumnType("varchar(20)");
                entity.Property(e => e.TipoContemplacao).IsRequired().HasColumnType("varchar(10)");
                entity.Property(e => e.DataContemplacao).HasColumnType("timestamp");
                entity.Property(e => e.Observacoes).HasColumnType("text");
                entity.Property(e => e.ValorCredito).HasColumnType("decimal(18,2)");
                entity.Property(e => e.ValorEntrada).HasColumnType("decimal(18,2)");
                entity.Property(e => e.ValorParcela).HasColumnType("decimal(18,2)");
                entity.Property(e => e.DataVenda).HasColumnType("timestamp");
                entity.Property(e => e.ValorVenda).HasColumnType("decimal(18,2)");

                // Relacionamentos
                entity.HasOne(c => c.Administradora)
                      .WithMany(a => a.CartasConsorcio)
                      .HasForeignKey(c => c.AdministradoraId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.Comprador)
                      .WithMany()
                      .HasForeignKey(c => c.CompradorId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.PropostaVenda)
                      .WithMany()
                      .HasForeignKey(c => c.PropostaVendaId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // Configuração da entidade DocumentoUsuario
            modelBuilder.Entity<DocumentoUsuario>(entity =>
            {
                entity.HasKey(e => e.Id);

                // Índices para performance
                entity.HasIndex(e => e.UsuarioId);
                entity.HasIndex(e => e.TipoDocumento);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => new { e.UsuarioId, e.TipoDocumento }).IsUnique();

                // Propriedades
                entity.Property(e => e.TipoDocumento).IsRequired().HasColumnType("varchar(50)");
                entity.Property(e => e.NomeArquivo).IsRequired().HasColumnType("varchar(255)");
                entity.Property(e => e.BlobUrl).IsRequired().HasColumnType("text");
                entity.Property(e => e.BlobName).IsRequired().HasColumnType("text");
                entity.Property(e => e.ContentType).IsRequired().HasColumnType("varchar(100)");
                entity.Property(e => e.Status).IsRequired().HasColumnType("varchar(20)").HasDefaultValue("pendente");
                entity.Property(e => e.ObservacoesValidacao).HasColumnType("text");

                entity.HasOne(d => d.Usuario)
                      .WithMany()
                      .HasForeignKey(d => d.UsuarioId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.ValidadoPor)
                      .WithMany()
                      .HasForeignKey(d => d.ValidadoPorId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // Configuração da entidade PropostaNegociacao
            modelBuilder.Entity<PropostaNegociacao>(entity =>
            {
                entity.HasKey(e => e.Id);

                // Índices para performance
                entity.HasIndex(e => e.CartaConsorcioId);
                entity.HasIndex(e => e.CompradorId);
                entity.HasIndex(e => e.Status);

                // Propriedades
                entity.Property(e => e.Status).IsRequired().HasColumnType("varchar(20)");
                entity.Property(e => e.MotivoCancelamento).HasColumnType("text");
                entity.Property(e => e.Agio).HasColumnType("decimal(18,2)");
                entity.Property(e => e.AnexoNomeArquivo).HasColumnType("varchar(255)");
                entity.Property(e => e.AnexoContentType).HasColumnType("varchar(100)");
                entity.Property(e => e.AnexoBlobUrl).HasColumnType("text");
                entity.Property(e => e.AnexoBlobName).HasColumnType("text");

                // Relacionamentos
                entity.HasOne(p => p.Carta)
                      .WithMany(c => c.Propostas)
                      .HasForeignKey(p => p.CartaConsorcioId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(p => p.Comprador)
                      .WithMany(u => u.PropostasComoComprador!)
                      .HasForeignKey(p => p.CompradorId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Parametros do sistema
            modelBuilder.Entity<ParametroSistema>(entity =>
            {
                entity.HasKey(p => p.Chave);
            });

            // Seed inicial de containers do Azure Blob Storage (Parametros)
            modelBuilder.Entity<ParametroSistema>().HasData(
                new ParametroSistema
                {
                    Chave = "Azure.Container.DocumentosUsuarios",
                    Valor = "documentos-usuarios",
                    Descricao = "Container para documentos de validação dos usuários (PII)"
                },
                new ParametroSistema
                {
                    Chave = "Azure.Container.AnexosPropostas",
                    Valor = "anexos-propostas",
                    Descricao = "Container para anexos de propostas"
                },
                new ParametroSistema
                {
                    Chave = "Azure.Container.AnexosCartas",
                    Valor = "anexos-cartas",
                    Descricao = "Container para anexos de cartas"
                },
                new ParametroSistema
                {
                    Chave = "Azure.Container.Default",
                    Valor = "documentos-usuarios",
                    Descricao = "Container padrão caso não definido"
                }
            );
        }
    }
}

