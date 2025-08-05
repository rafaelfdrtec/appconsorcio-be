using AppConsorciosMvp.Data;
using Microsoft.EntityFrameworkCore;

namespace AppConsorciosMvp.Extensions
{
    /// <summary>
    /// Extensões para aplicar migrações automaticamente
    /// </summary>
    public static class MigrationExtensions
    {
        /// <summary>
        /// Aplica migrações automaticamente na inicialização
        /// </summary>
        /// <param name="app">Aplicação web</param>
        /// <returns>A aplicação web</returns>
        public static WebApplication AplicarMigracoes(this WebApplication app)
        {
            // Aplicar migrações apenas em ambiente de produção ou desenvolvimento
            if (app.Environment.IsProduction() || app.Environment.IsDevelopment())
            {
                using var scope = app.Services.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                try
                {
                    // Verificar se as migrações existem
                    var migracoesExistentes = dbContext.Database.GetPendingMigrations();

                    if (!migracoesExistentes.Any())
                    {
                        // Se não houver migrações, crie o banco diretamente
                        app.Logger.LogInformation("Não há migrações disponíveis. Criando banco de dados diretamente...");
                        dbContext.Database.EnsureCreated();
                        app.Logger.LogInformation("Banco de dados criado com sucesso!");
                    }
                    else
                    {
                        // Aplicar migrações existentes
                        app.Logger.LogInformation("Aplicando migrações automaticamente...");
                        dbContext.Database.Migrate();
                        app.Logger.LogInformation("Migrações aplicadas com sucesso!");
                    }
                }
                catch (Exception ex)
                {
                    app.Logger.LogError(ex, "Erro ao aplicar migrações do banco de dados. Tentando criar o banco diretamente...");

                    try
                    {
                        // Em caso de erro, tente criar o banco diretamente
                        dbContext.Database.EnsureCreated();
                        app.Logger.LogInformation("Banco de dados criado com sucesso após falha nas migrações!");
                    }
                    catch (Exception innerEx)
                    {
                        app.Logger.LogError(innerEx, "Erro ao criar banco de dados diretamente.");
                    }
                }
            }

            return app;
        }
    }
}
