using System.Text;
using AppConsorciosMvp.Data;
using AppConsorciosMvp.Extensions;
using AppConsorciosMvp.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Npgsql.EntityFrameworkCore.PostgreSQL;

var builder = WebApplication.CreateBuilder(args);

// Obter configurações
var configuration = builder.Configuration;
var connectionString = configuration.GetConnectionString("DefaultConnection") ?? 
                      throw new InvalidOperationException("String de conexão 'DefaultConnection' não encontrada.");
var jwtSecret = configuration["JWT:Secret"] ?? 
               throw new InvalidOperationException("JWT Secret não configurado");

// Adicionar serviços ao container
builder.Services.AddControllers();

// Registrar serviços customizados
builder.Services.AddScoped<AzureBlobService>();

// Configurar banco de dados
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Registrar serviços personalizados
builder.Services.AddScoped<PasswordHashService>();
builder.Services.AddScoped<TokenService>();

// Configurar autenticação JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

// Configurar CORS para desenvolvimento
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Configurar Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API de Consórcios",
        Version = "v1",
        Description = "API para transação de cartas de consórcio contempladas"
    });

    // Configurar autenticação no Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando o esquema Bearer. Exemplo: 'Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Aplicar migrações automaticamente
app.AplicarMigracoes();

// Configurar o pipeline de requisições HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Consórcios v1");
        c.RoutePrefix = string.Empty; // Para servir a UI do Swagger na raiz
    });
    app.UseCors("AllowAll");
}
else
{
    // Em produção, também habilitamos o Swagger, mas com caminho específico
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Consórcios v1");
        c.RoutePrefix = "docs"; // Em produção, acessível via /docs
    });

    // Configurações de segurança para produção
    app.UseHsts();
}

app.UseHttpsRedirection();

// Adicionar middleware de autenticação e autorização
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();