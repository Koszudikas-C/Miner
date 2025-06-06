#!/bin/bash

# Variáveis
DOMAIN="monerokoszudikas"  # Substitua pelo seu domínio DuckDNS (sem .duckdns.org)
TOKEN="20e08e21-534b-4b4e-9a4d-7148b48a9ef7"      # Substitua pelo seu token do DuckDNS
TXT_RECORD="Wr4kqakZNPIirrc1vBCDVsBO0V7zzDAlhmpuu0tI9zE"   # Substitua pelo valor do TXT fornecido pelo Certbot

# URL da API do DuckDNS
URL="https://www.duckdns.org/update?domains=$DOMAIN&token=$TOKEN&txt=$TXT_RECORD&verbose=true"

# Faz a solicitação à API
RESPONSE=$(curl -s "$URL")

# Exibe a resposta
echo "Resposta da API: $RESPONSE"