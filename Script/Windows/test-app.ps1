param (
    [Parameter(Mandatory = $true)]
    [string]$Binario,

    [Parameter(Mandatory = $true)]
    [int]$Vezes,

    [Parameter(Mandatory = $true)]
    [int]$Delay = 1  
)

# Verifica se o binário existe
if (-not (Test-Path $Binario)) {
    Write-Error "Erro: O binário '$Binario' não existe."
    exit 1
}

# Verifica se o número de vezes é válido
if ($Vezes -le 0) {
    Write-Error "Erro: O número de vezes deve ser um inteiro positivo."
    exit 1
}

# Verifica se o delay é válido
if ($Delay -lt 0) {
    Write-Error "Erro: O delay deve ser um número inteiro não negativo."
    exit 1
}

# Loop para executar o binário X vezes
for ($i = 1; $i -le $Vezes; $i++) {
    Write-Host "Executando o binário '$Binario' (execução $i de $Vezes)..."

    try {
        Start-Process -FilePath $Binario -NoNewWindow -ErrorAction Stop
    }
    catch {
        Write-Error "Erro: Falha ao executar o binário na execução $i. $_"
        exit 1
    }

    if ($i -lt $Vezes) {
        Write-Host "Aguardando $Delay segundo(s) antes da próxima execução..."
        Start-Sleep -Seconds $Delay
    }
}

Write-Host "Binário executado $Vezes vezes com sucesso (com intervalo de $Delay segundo(s) entre cada execução)."
