#!/bin/bash

echo "Atualizando variáveis de ambiente no /etc/environment (sem duplicar)..."

# Backup
sudo cp /etc/environment /etc/environment.bak

# Define as variáveis que queremos garantir no arquivo
declare -A vars
vars["SERVICE_NAME_API_MINE"]="bsnti7a6yvf7m6ag3ryoolxspvv6j4ps3kvqa47eywoeaumv4q2gasyd.onion"
vars["SERVICE_NAME_WORK_SERVICE"]="3wfqc53bbzi4uytqxsiohjzc26oj5o3xvulqytdxz5litdlwuyw66mid.onion"
vars["SERVICE_NAME_REMOTE_BLOCK"]="ee2vg7qiu4wr3p6wf27zxfrvpmcddvkutocovwnyho4el2obi4vbx2ad"
vars["SERVICE_NAME_REMOTE_BLOCK_SSL"]="monerokoszudikas.duckdns.org"
vars["SERVICE_NAME_PROXY_MINE"]="dheq2yivibimsydghgqxnk7iahchjpmzcpijzxrzfmn5lbd6b5yfgdyd"
vars["CERTIFICATE_PATH"]="/etc/ssl/certs/Monerokoszudikas.pfx"
vars["CERTIFICATE_PASSWORD"]="88199299"
vars["PROXY_HOST"]="127.0.0.1"
vars["PROXY_PORT"]="9050"
vars["REMOTE_PORT"]="80"
vars["REMOTE_USERNAME"]="OK"
vars["REMOTE_PASSWORD"]="DqzTQ@dsaweq@4"

# Cria um novo arquivo temporário ignorando as variáveis duplicadas
temp_file=$(mktemp)

# Remove linhas duplicadas
grep -v -E "^($(IFS="|"; echo "${!vars[*]}"))=" /etc/environment > "$temp_file"

# Adiciona as novas definições
for key in "${!vars[@]}"; do
    echo "$key=\"${vars[$key]}\"" >> "$temp_file"
done

# Substitui o arquivo original
sudo mv "$temp_file" /etc/environment
sudo chmod 644 /etc/environment

source /etc/environment

echo "Atualização concluída. Reinicie ou faça logout/login para aplicar."
