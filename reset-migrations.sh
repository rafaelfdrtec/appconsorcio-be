#!/bin/bash

# Script para redefinir e recriar migrações para PostgreSQL, dropando o banco antes

set -e

# Verificar se o dotnet ef está instalado
if ! dotnet tool list --global | grep -q "dotnet-ef"; then
    echo "Instalando ferramenta dotnet-ef..."
    dotnet tool install --global dotnet-ef
fi

# Navegar para o diretório do projeto
cd ./AppConsorciosMvp

# Dropar o banco de dados (sem prompt)
echo "Dropando o banco de dados..."
dotnet ef database drop -f || true

# Garantir pastas
mkdir -p ./Data/Migrations

# Remover migrações antigas
echo "Removendo migrações existentes..."
rm -f ./Data/Migrations/*.cs || true

# Criar nova migração inicial
echo "Criando nova migração inicial para PostgreSQL..."
dotnet ef migrations add InitialPostgres

# Aplicar as migrações no banco vazio
echo "Aplicando a migração inicial no banco de dados..."
dotnet ef database update

echo -e "\033[0;32mBanco zerado e recriado com sucesso!\033[0m"

# Voltar para o diretório original
cd ..
