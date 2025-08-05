FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Instalar dependências do PostgreSQL no contêiner base
RUN apt-get update && apt-get install -y apt-transport-https && \
    apt-get install -y postgresql-client

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["AppConsorciosMvp/AppConsorciosMvp.csproj", "AppConsorciosMvp/"]
RUN dotnet restore "AppConsorciosMvp/AppConsorciosMvp.csproj"
COPY . .
WORKDIR "/src/AppConsorciosMvp"
RUN dotnet build "./AppConsorciosMvp.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./AppConsorciosMvp.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Instalar ferramentas EF Core para migrações
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Adicionar script de inicialização para aplicar migrações
COPY --from=build /src/AppConsorciosMvp/entrypoint.sh /app/entrypoint.sh
RUN chmod +x /app/entrypoint.sh

ENTRYPOINT ["/app/entrypoint.sh"]
