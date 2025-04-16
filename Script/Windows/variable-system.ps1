Write-Host "Exportando variáveis de ambiente globalmente para o PowerShell..."

# Variáveis de sessão
$env:SERVICE_NAME_API_MINE = "bsnti7a6yvf7m6ag3ryoolxspvv6j4ps3kvqa47eywoeaumv4q2gasyd.onion"
$env:SERVICE_NAME_WORK_SERVICE = "3wfqc53bbzi4uytqxsiohjzc26oj5o3xvulqytdxz5litdlwuyw66mid.onion"
$env:SERVICE_NAME_REMOTEBLOCK = "ee2vg7qiu4wr3p6wf27zxfrvpmcddvkutocovwnyho4el2obi4vbx2ad"
$env:SERVICE_NAME_PROXY_MINE = "dheq2yivibimsydghgqxnk7iahchjpmzcpijzxrzfmn5lbd6b5yfgdyd"
$env:CERTIFICATE_PATH = "/etc/ssl/certs/Monerokoszudikas.pfx"
$env:CERTIFICATE_PASSWORD = "88199299"

# Novas variáveis
$env:PROXY_HOST = "127.0.0.1"
$env:PROXY_PORT = "9050"
$env:REMOTE_ONION_HOST = "3wfqc53bbzi4uytqxsiohjzc26oj5o3xvulqytdxz5litdlwuyw66mid.onion"
$env:REMOTE_PORT = "80"
$env:REMOTE_STATUS_OK = "OK"
$env:REMOTE_SECRET = "DqzTQ!%%@dsaweq@!4"

# Persistir no $PROFILE
$profileContent = @'
$env:SERVICE_NAME_API_MINE = "bsnti7a6yvf7m6ag3ryoolxspvv6j4ps3kvqa47eywoeaumv4q2gasyd.onion"
$env:SERVICE_NAME_WORK_SERVICE = "3wfqc53bbzi4uytqxsiohjzc26oj5o3xvulqytdxz5litdlwuyw66mid.onion"
$env:SERVICE_NAME_REMOTEBLOCK = "ee2vg7qiu4wr3p6wf27zxfrvpmcddvkutocovwnyho4el2obi4vbx2ad"
$env:SERVICE_NAME_PROXY_MINE = "dheq2yivibimsydghgqxnk7iahchjpmzcpijzxrzfmn5lbd6b5yfgdyd"
$env:CERTIFICATE_PATH = "/etc/ssl/certs/Monerokoszudikas.pfx"
$env:CERTIFICATE_PASSWORD = "88199299"

$env:PROXY_HOST = "127.0.0.1"
$env:PROXY_PORT = "9050"
$env:REMOTE_ONION_HOST = "3wfqc53bbzi4uytqxsiohjzc26oj5o3xvulqytdxz5litdlwuyw66mid.onion"
$env:REMOTE_PORT = "80"
$env:REMOTE_STATUS_OK = "OK"
$env:REMOTE_SECRET = "DqzTQ!%%@dsaweq@!4"
'@

Add-Content -Path $PROFILE -Value $profileContent

# Carrega o perfil na sessão atual
. $PROFILE

Write-Host "Todas as variáveis de ambiente foram exportadas para a sessão atual e adicionadas ao perfil."
