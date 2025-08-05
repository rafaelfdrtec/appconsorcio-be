#!/bin/bash

# Instalar cliente PostgreSQL para verificação de conexão
echo "Instalando cliente PostgreSQL..."
apt-get update && apt-get install -y postgresql-client

# Esperar o PostgreSQL iniciar
echo "Aguardando PostgreSQL iniciar..."
sleep 10s

# Verificar se o PostgreSQL está pronto
echo "Verificando conexão com PostgreSQL..."

# Tentativas de conexão
COUNT=0
MAX_TRIES=30
SLEEP_TIME=2

until pg_isready -h db -U postgres > /dev/null 2>&1 || [ $COUNT -eq $MAX_TRIES ]; do
  echo "Aguardando PostgreSQL... ($COUNT/$MAX_TRIES)"
  sleep $SLEEP_TIME
  COUNT=$((COUNT+1))
done

if [ $COUNT -eq $MAX_TRIES ]; then
  echo "Não foi possível conectar ao PostgreSQL após $MAX_TRIES tentativas!"
else
  echo "PostgreSQL está pronto!"
fi

# Executar o aplicativo
echo "Iniciando a aplicação..."
exec dotnet AppConsorciosMvp.dll
