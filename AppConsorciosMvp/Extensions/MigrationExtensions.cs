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
                    app.Logger.LogInformation("Aplicando migrações automaticamente...");
                    dbContext.Database.Migrate();
                    app.Logger.LogInformation("Migrações aplicadas com sucesso!");
                }
                catch (Exception ex)
                {
                    app.Logger.LogError(ex, "Erro ao aplicar migrações do banco de dados.");
                }
            }

            return app;
        }
    }
}
