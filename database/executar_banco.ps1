param(
    [string]$Servidor = "localhost",
    [int]$Porta = 3306,
    [string]$Usuario = "root",
    [switch]$Sim,           # não pergunta antes de apagar o banco retrogames
    [switch]$SoEstrutura    # executa só 01, 02 e 03 (sem consultas e testes)
)

$ErrorActionPreference = "Stop"
$pasta = $PSScriptRoot

if (-not (Get-Command mysql -ErrorAction SilentlyContinue)) {
    throw "O cliente 'mysql' não foi encontrado no PATH. Adicione a pasta bin do MySQL 8 ao PATH e tente de novo."
}

$senhaDefinidaAqui = $false
if (-not $env:MYSQL_PWD) {
    $seguro = Read-Host "Senha do MySQL para o usuario '$Usuario'" -AsSecureString
    $bstr = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($seguro)
    try { $env:MYSQL_PWD = [Runtime.InteropServices.Marshal]::PtrToStringBSTR($bstr) }
    finally { [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($bstr) }
    $senhaDefinidaAqui = $true
}

function Invoke-Mysql {
    param([string]$Arquivo, [string[]]$Extra = @())
    $argumentos = @("--host=$Servidor", "--port=$Porta", "--user=$Usuario",
                    "--default-character-set=utf8mb4", "--table") + $Extra
    # O arquivo é entregue ao mysql como bytes (stdin), o que preserva os acentos (UTF-8).
    $p = Start-Process -FilePath "mysql" -ArgumentList $argumentos `
            -RedirectStandardInput $Arquivo -NoNewWindow -Wait -PassThru
    return $p.ExitCode
}

function Invoke-Etapa {
    param([string]$Titulo, [string]$Nome, [switch]$IgnorarErros)
    Write-Host ""
    Write-Host "=== $Titulo ($Nome) ===" -ForegroundColor Cyan
    $extra = @()
    if ($IgnorarErros) { $extra = @("--force") }
    $codigo = Invoke-Mysql -Arquivo (Join-Path $pasta $Nome) -Extra $extra
    if ($codigo -ne 0 -and -not $IgnorarErros) {
        throw "O script $Nome terminou com erro (codigo $codigo). Corrija e execute de novo."
    }
}

try {
    if (-not $Sim) {
        $r = Read-Host "Isto APAGA o banco 'retrogames' e o recria do zero. Continuar? (s/N)"
        if ($r -notin @("s", "S", "sim", "SIM")) { Write-Host "Cancelado."; return }
    }

    $tmp = Join-Path ([IO.Path]::GetTempPath()) "retrogames_drop.sql"
    Set-Content -Path $tmp -Value "DROP DATABASE IF EXISTS retrogames;" -Encoding ASCII
    Write-Host "=== Apagando o banco retrogames (se existir) ===" -ForegroundColor Cyan
    $codigo = Invoke-Mysql -Arquivo $tmp
    Remove-Item $tmp -ErrorAction SilentlyContinue
    if ($codigo -ne 0) { throw "Não foi possível conectar ao MySQL ou apagar o banco (codigo $codigo)." }

    Invoke-Etapa "Etapa 3 - criar tabelas"          "01_create_tables.sql"
    Invoke-Etapa "Etapa 3 - corrigir estrutura"     "02_corrigir_estrutura.sql"
    Invoke-Etapa "Etapa 4 - dados de exemplo"       "03_dados_exemplo.sql"

    if (-not $SoEstrutura) {
        Invoke-Etapa "Etapa 5 - consultas"               "04_consultas.sql" -IgnorarErros
        Write-Host ""
        Write-Host "Os erros 1452, 1062, 1451 e 1048 abaixo sao ESPERADOS; o teste 5 deve ser aceito." -ForegroundColor Yellow
        Invoke-Etapa "Etapa 5 - testes de integridade"   "05_testes_integridade.sql" -IgnorarErros
    }

    Write-Host ""
    Write-Host "Concluido." -ForegroundColor Green
}
finally {
    if ($senhaDefinidaAqui) { Remove-Item Env:\MYSQL_PWD -ErrorAction SilentlyContinue }
}