#!/bin/bash

# Script para redefinir e recriar migrações para PostgreSQL

# Verificar se o dotnet ef está instalado
if ! dotnet tool list --global | grep -q "dotnet-ef"; then
    echo "Instalando ferramenta dotnet-ef..."
    dotnet tool install --global dotnet-ef
    if [ $? -ne 0 ]; then
        echo "Erro ao instalar dotnet-ef. Saindo..."
        exit 1
    fi
fi

# Navegar para o diretório do projeto
cd ./AppConsorciosMvp

# Verificar se a pasta Data/Migrations existe, criar se não existir
if [ ! -d "./Data" ]; then
    echo "Criando pasta Data..."
    mkdir -p ./Data
fi

if [ ! -d "./Data/Migrations" ]; then
    echo "Criando pasta Data/Migrations..."
    mkdir -p ./Data/Migrations
fi

# Remover migrações antigas
echo "Removendo migrações existentes..."
rm -f ./Data/Migrations/*.cs

# Criar nova migração inicial
echo "Criando nova migração inicial para PostgreSQL..."
dotnet ef migrations add InitialPostgres

if [ $? -eq 0 ]; then
    echo "\033[0;32mMigração criada com sucesso!\033[0m"
    echo "\033[0;32mAgora você pode aplicar a migração com: dotnet ef database update\033[0m"
else
    echo "\033[0;31mErro ao criar migração.\033[0m"
fi

# Voltar para o diretório original
cd ..
