# Script para atualizar o banco de dados

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

# Aplicar as migrações
Write-Host "Aplicando migrações no banco de dados..."
dotnet ef database update

if ($LASTEXITCODE -eq 0) {
    Write-Host "Banco de dados atualizado com sucesso!" -ForegroundColor Green
} else {
    Write-Host "Erro ao atualizar o banco de dados." -ForegroundColor Red
}

# Voltar para o diretório original
Set-Location -Path ".."
