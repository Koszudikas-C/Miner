#!/bin/bash

NETWORK_NAME="mine"
VOLUME_NAME="shared-data"

echo "🔄 Criando a rede Docker..."
sudo docker network create $NETWORK_NAME

echo "Criando volume compartihlado"
sudo docker volume create $VOLUME_NAME

echo "✅ Rede criada ${NETWORK_NAME} é volume ${VOLUME_NAME}. Agora iniciando os serviços..."

sudo docker compose -f $APP_PATH_PRIMARY_DOCKER/Proxy-reverse/Nginx/nginx.yaml up --build -d

#sudo docker compose -f $APP_PATH_PRIMARY_DOCKER/Certificate/Certbot/certbot.yaml up --build -d

sudo docker compose -f $APP_PATH_PRIMARY_DOCKER/API/RemoteWorkServiceClient/api-work-client.yaml up --build -d

sudo docker compose -f $APP_PATH_PRIMARY_DOCKER/Proxy-reverse/Tor/tor.yaml up --build -d

echo $APP_PATH_PRIMARY_DOCKER
echo "🚀 Todos os serviços foram iniciados!"
