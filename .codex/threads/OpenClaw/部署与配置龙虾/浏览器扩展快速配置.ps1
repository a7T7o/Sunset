param(
  [ValidateSet('chrome', 'edge')]
  [string]$Browser = 'chrome'
)

$ErrorActionPreference = 'Stop'

$configPath = Join-Path $env:USERPROFILE '.openclaw\openclaw.json'
$extensionPath = Join-Path $env:USERPROFILE '.openclaw\browser\chrome-extension'
$cfg = Get-Content $configPath -Raw | ConvertFrom-Json
$token = [string]$cfg.gateway.auth.token

if ([string]::IsNullOrWhiteSpace($token)) {
  throw "未找到 gateway.auth.token，请先检查 $configPath"
}

if (-not (Test-Path $extensionPath)) {
  throw "未找到扩展目录：$extensionPath"
}

$browserExe = $null
$extensionsUrl = $null
$relayPort = $null

if ($Browser -eq 'chrome') {
  $relayPort = 18792
  $extensionsUrl = 'chrome://extensions'
  $candidates = @(
    "$env:ProgramFiles\Google\Chrome\Application\chrome.exe",
    "${env:ProgramFiles(x86)}\Google\Chrome\Application\chrome.exe"
  )
} else {
  $relayPort = 18793
  $extensionsUrl = 'edge://extensions'
  $candidates = @(
    "${env:ProgramFiles(x86)}\Microsoft\Edge\Application\msedge.exe",
    "$env:ProgramFiles\Microsoft\Edge\Application\msedge.exe"
  )
}

foreach ($candidate in $candidates) {
  if ($candidate -and (Test-Path $candidate)) {
    $browserExe = $candidate
    break
  }
}

if (-not $browserExe) {
  throw "未找到 $Browser 浏览器可执行文件。"
}

Set-Clipboard -Value $token
Start-Process explorer.exe $extensionPath | Out-Null
Start-Process -FilePath $browserExe -ArgumentList $extensionsUrl | Out-Null

Write-Host ""
Write-Host "OpenClaw 浏览器扩展快速配置已准备好。" -ForegroundColor Green
Write-Host "浏览器：" $Browser
Write-Host "扩展目录：" $extensionPath
Write-Host "Relay 端口：" $relayPort
Write-Host "Gateway token 已复制到剪贴板。"
Write-Host ""
Write-Host "你接下来只需要："
Write-Host "1. 在打开的扩展页开启 Developer mode"
Write-Host "2. 选择 Load unpacked"
Write-Host "3. 选择上面已打开的扩展目录"
Write-Host "4. 打开扩展 Options"
Write-Host "5. 端口填 $relayPort"
Write-Host "6. 粘贴剪贴板里的 token 并保存"
