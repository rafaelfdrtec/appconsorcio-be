#!/bin/bash

# Script para atualizar o banco de dados

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

# Aplicar as migrações
echo "Aplicando migrações no banco de dados..."
dotnet ef database update

if [ $? -eq 0 ]; then
    echo "\033[0;32mBanco de dados atualizado com sucesso!\033[0m"
else
    echo "\033[0;31mErro ao atualizar o banco de dados.\033[0m"
fi

# Voltar para o diretório original
cd ..
