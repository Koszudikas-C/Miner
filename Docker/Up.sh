#!/bin/bash

NETWORK_NAME="mine"

echo "🔄 Criando a rede Docker..."
sudo docker network create $NETWORK_NAME

echo "✅ Rede criada. Agora iniciando os serviços..."

sudo docker-compose -f $APP_PATH_PRIMARY_DOCKER/Proxy-reverse/nginx.yaml up --build -d

sudo docker-compose -f $APP_PATH_PRIMARY_DOCKER/API/RemoteWorkServiceClient/api-work-client.yaml up --build -d

sudo docker-compose -f $APP_PATH_PRIMARY_DOCKER/Proxy-reverse/Tor/tor.yaml up --build -d


echo $APP_PATH_PRIMARY_DOCKER
echo "🚀 Todos os serviços foram iniciados!"
