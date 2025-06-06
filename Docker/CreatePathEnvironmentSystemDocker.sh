#!/bin/bash

VAR_NAME="APP_PATH_PRIMARY_DOCKER"
VAR_VALUE=$(pwd)
SHELL_CONFIGS=(~/.bashrc ~/.zshrc)

remove_var() {
    echo "🚮 Removendo variável $VAR_NAME..."

    for file in "${SHELL_CONFIGS[@]}"; do
        sed -i "/export $VAR_NAME=/d" "$file"
    done

    unset "$VAR_NAME"
    echo "✅ Variável $VAR_NAME removida!"
}

if [[ "$1" == "--delete" ]]; then
    remove_var
    exit 0
fi

if printenv | grep -q "^$VAR_NAME="; then
    echo "⚠️ A variável $VAR_NAME já está definida como $(printenv $VAR_NAME)"
    echo "Use './CreatePathEnvironmentSystemDocker.sh --delete' para remover antes de adicionar novamente."
    exit 1
fi


export $VAR_NAME=$VAR_VALUE

for file in "${SHELL_CONFIGS[@]}"; do
    echo "export $VAR_NAME=$VAR_VALUE" >> "$file"
done

echo "✅ Variável de ambiente $VAR_NAME definida com sucesso!"
echo "📂 Valor: $VAR_VALUE"

source ~/.bashrc || source ~/.zshrc
