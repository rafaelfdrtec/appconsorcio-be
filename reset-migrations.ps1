# Script para redefinir e recriar migrações para PostgreSQL

# Verificar se o dotnet ef está instalado
$efInstalled = dotnet tool list --global | Select-String "dotnet-ef"
if (-not $efInstalled) {
    Write-Host "Instalando ferramenta dotnet-ef..."
    dotnet tool install --global dotnet-ef
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Erro ao instalar dotnet-ef. Saindo..."
        exit 1
    }
}

# Navegar para o diretório do projeto
Set-Location -Path "./AppConsorciosMvp"

# Verificar se a pasta Data/Migrations existe, criar se não existir
if (-not (Test-Path -Path "./Data")) {
    Write-Host "Criando pasta Data..."
    New-Item -Path "./Data" -ItemType Directory
}

if (-not (Test-Path -Path "./Data/Migrations")) {
    Write-Host "Criando pasta Data/Migrations..."
    New-Item -Path "./Data/Migrations" -ItemType Directory
}

# Remover migrações antigas
Write-Host "Removendo migrações existentes..."
Remove-Item -Path "./Data/Migrations/*.cs" -Force -ErrorAction SilentlyContinue

# Criar nova migração inicial
Write-Host "Criando nova migração inicial para PostgreSQL..."
dotnet ef migrations add InitialPostgres

if ($LASTEXITCODE -eq 0) {
    Write-Host "Migração criada com sucesso!" -ForegroundColor Green
    Write-Host "Agora você pode aplicar a migração com: dotnet ef database update" -ForegroundColor Green
} else {
    Write-Host "Erro ao criar migração." -ForegroundColor Red
}

# Voltar para o diretório original
Set-Location -Path ".."
