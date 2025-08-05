#!/bin/bash

# Esperar o SQL Server iniciar
echo "Aguardando SQL Server iniciar..."
sleep 30s

# Executar o aplicativo
echo "Iniciando a aplicação..."
exec dotnet AppConsorciosMvp.dll
