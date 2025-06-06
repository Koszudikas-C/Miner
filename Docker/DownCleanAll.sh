#!/bin/bash

echo "🧹 Limpando tudo no Docker..."

echo "⛔ Parando todos os containers..."
sudo docker stop $(docker ps -aq) 2>/dev/null

echo "🗑️ Removendo todos os containers..."
sudo docker rm $(docker ps -aq) 2>/dev/null

echo "🖼️ Removendo todas as imagens..."
sudo docker rmi -f $(docker images -aq) 2>/dev/null

echo "📦 Removendo todos os volumes..."
sudo docker volume rm $(docker volume ls -q) 2>/dev/null

echo "🌐 Removendo redes criadas pelo usuário..."
sudo docker network rm $(docker network ls | grep "bridge" | awk '{print $1}') 2>/dev/null
sudo docker network rm $(docker network ls | grep -v "bridge\|host\|none" | awk '{print $1}') 2>/dev/null

echo "✅ Docker limpo com sucesso!"
