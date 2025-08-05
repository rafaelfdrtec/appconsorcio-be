using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace AppConsorciosMvp.Data.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("AppConsorciosMvp.Models.CartaConsorcio", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");

                SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                b.Property<DateTime>("CriadoEm")
                    .HasColumnType("datetime2");

                b.Property<string>("Descricao")
                    .HasColumnType("nvarchar(max)");

                b.Property<bool>("EhVerificado")
                    .HasColumnType("bit");

                b.Property<int>("ParcelasPagas")
                    .HasColumnType("int");

                b.Property<int>("ParcelasTotais")
                    .HasColumnType("int");

                b.Property<string>("Status")
                    .IsRequired()
                    .HasColumnType("nvarchar(450)");

                b.Property<string>("TipoBem")
                    .IsRequired()
                    .HasColumnType("nvarchar(450)");

                b.Property<decimal>("ValorCredito")
                    .HasColumnType("decimal(18,2)");

                b.Property<decimal>("ValorEntrada")
                    .HasColumnType("decimal(18,2)");

                b.Property<decimal>("ValorParcela")
                    .HasColumnType("decimal(18,2)");

                b.Property<int>("VendedorId")
                    .HasColumnType("int");

                b.HasKey("Id");

                b.HasIndex("Status");

                b.HasIndex("TipoBem");

                b.HasIndex("VendedorId");

                b.ToTable("CartasConsorcio");
            });

            modelBuilder.Entity("AppConsorciosMvp.Models.Usuario", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");

                SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                b.Property<string>("Email")
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnType("nvarchar(100)");

                b.Property<bool>("EhVerificado")
                    .HasColumnType("bit");

                b.Property<string>("Nome")
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnType("nvarchar(100)");

                b.Property<string>("Papel")
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");

                b.Property<string>("SenhaHash")
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");

                b.HasKey("Id");

                b.HasIndex("Email")
                    .IsUnique();

                b.ToTable("Usuarios");
            });

            modelBuilder.Entity("AppConsorciosMvp.Models.CartaConsorcio", b =>
            {
                b.HasOne("AppConsorciosMvp.Models.Usuario", "Vendedor")
                    .WithMany("CartasConsorcio")
                    .HasForeignKey("VendedorId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.Navigation("Vendedor");
            });

            modelBuilder.Entity("AppConsorciosMvp.Models.Usuario", b =>
            {
                b.Navigation("CartasConsorcio");
            });
        }
    }
}
