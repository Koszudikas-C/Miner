Write-Host "Atualizando variáveis de ambiente (sem duplicar)..."

# Define as variáveis que queremos garantir no sistema
$vars = @{
    "SERVICE_NAME_API_MINE"       = "bsnti7a6yvf7m6ag3ryoolxspvv6j4ps3kvqa47eywoeaumv4q2gasyd.onion"
    "SERVICE_NAME_WORK_SERVICE"  = "3wfqc53bbzi4uytqxsiohjzc26oj5o3xvulqytdxz5litdlwuyw66mid.onion"
    "SERVICE_NAME_REMOTE_BLOCK"  = "ee2vg7qiu4wr3p6wf27zxfrvpmcddvkutocovwnyho4el2obi4vbx2ad"
    "SERVICE_NAME_REMOTE_BLOCK_SSL" = "monerokoszudikas.duckdns.org"
    "SERVICE_NAME_PROXY_MINE"    = "dheq2yivibimsydghgqxnk7iahchjpmzcpijzxrzfmn5lbd6b5yfgdyd"
    "CERTIFICATE_PATH"           = "C:\etc\ssl\certs\Monerokoszudikas.pfx"
    "CERTIFICATE_PASSWORD"       = "88199299"
    "PROXY_HOST"                 = "127.0.0.1"
    "PROXY_PORT"                 = "9050"
    "REMOTE_PORT"                = "80"
    "REMOTE_USERNAME_TOR"        = "Koszudikas"
    "REMOTE_PASSWORD_TOR"        = "DqzTQ@dsaw"
}

# Backup das variáveis atuais
$backupFile = "$env:SystemRoot\System32\drivers\etc\environment.bak"
Copy-Item -Path "HKLM:\SYSTEM\CurrentControlSet\Control\Session Manager\Environment" -Destination $backupFile -Recurse -Force

# Define ou atualiza as variáveis de ambiente de sistema
foreach ($key in $vars.Keys) {
    [Environment]::SetEnvironmentVariable($key, $vars[$key], [EnvironmentVariableTarget]::Machine)
}

Write-Host "Atualização concluída."

Write-Host "Atenção: reinicie ou faça logout/login para aplicar as alterações no ambiente."
