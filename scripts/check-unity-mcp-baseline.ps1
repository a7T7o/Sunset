param(
    [string]$ProjectRoot = "D:\Unity\Unity_learning\Sunset",
    [string]$CodexHome = "C:\Users\aTo\.codex",
    [int]$Port = 8888
)

$ErrorActionPreference = "Stop"

function Write-Section {
    param([string]$Title)
    Write-Output ""
    Write-Output "== $Title =="
}

function Test-ConfigOnlyUnityMcp {
    param([string]$ConfigPath)

    if (-not (Test-Path $ConfigPath)) {
        return [pscustomobject]@{
            Exists = $false
            UnityBlock = $false
            LegacyBlock = $false
            Endpoint = $null
        }
    }

    $content = Get-Content -Raw $ConfigPath
    $unityBlock = $content -match '(?m)^\[mcp_servers\.unityMCP\]\s*$'
    $legacyBlock = $content -match '(?m)^\[mcp_servers\.mcp-unity\]\s*$'
    $endpoint = $null
    if ($content -match '(?m)^url\s*=\s*"([^"]+)"\s*$') {
        $endpoint = $Matches[1]
    }

    return [pscustomobject]@{
        Exists = $true
        UnityBlock = $unityBlock
        LegacyBlock = $legacyBlock
        Endpoint = $endpoint
    }
}

$configPath = Join-Path $CodexHome "config.toml"
$pidPath = Join-Path $ProjectRoot "Library\MCPForUnity\RunState\mcp_http_$Port.pid"
$terminalPath = Join-Path $ProjectRoot "Library\MCPForUnity\TerminalScripts\mcp-terminal.cmd"

$config = Test-ConfigOnlyUnityMcp -ConfigPath $configPath
$listener = Get-NetTCPConnection -LocalPort $Port -State Listen -ErrorAction SilentlyContinue | Select-Object -First 1
$pidValue = if (Test-Path $pidPath) { (Get-Content $pidPath -ErrorAction SilentlyContinue | Select-Object -First 1) } else { $null }

Write-Section "Codex Config"
Write-Output "config_path: $configPath"
Write-Output "config_exists: $($config.Exists)"
Write-Output "has_unityMCP_block: $($config.UnityBlock)"
Write-Output "has_legacy_mcp_unity_block: $($config.LegacyBlock)"
Write-Output "configured_endpoint: $($config.Endpoint)"

Write-Section "Listener"
if ($listener) {
    Write-Output "listener: active"
    Write-Output "port: $($listener.LocalPort)"
    Write-Output "owning_process: $($listener.OwningProcess)"
}
else {
    Write-Output "listener: missing"
}

Write-Section "Pidfile"
Write-Output "pidfile_path: $pidPath"
Write-Output "pidfile_exists: $(Test-Path $pidPath)"
Write-Output "pidfile_value: $pidValue"

Write-Section "Terminal Script"
Write-Output "terminal_script_path: $terminalPath"
Write-Output "terminal_script_exists: $(Test-Path $terminalPath)"
if (Test-Path $terminalPath) {
    Get-Content $terminalPath | Select-Object -First 5
}

Write-Section "Summary"
$issues = @()
if (-not $config.Exists) { $issues += "config_missing" }
if (-not $config.UnityBlock) { $issues += "unityMCP_missing" }
if ($config.LegacyBlock) { $issues += "legacy_mcp_unity_present" }
if ($config.Endpoint -ne "http://127.0.0.1:$Port/mcp") { $issues += "unexpected_endpoint" }
if (-not $listener) { $issues += "listener_missing" }
if (-not (Test-Path $pidPath)) { $issues += "pidfile_missing" }

if ($issues.Count -eq 0) {
    Write-Output "baseline_status: pass"
}
else {
    Write-Output "baseline_status: fail"
    Write-Output "issues: $($issues -join ', ')"
}
