param(
    [string]$RepoRoot = "D:\Unity\Unity_learning\Sunset"
)

$ErrorActionPreference = "Stop"

$targets = @()

$draftRoot = Join-Path $RepoRoot ".codex\drafts"
if (Test-Path $draftRoot) {
    $targets += Get-ChildItem $draftRoot -Recurse -File -Filter "*.md" -ErrorAction SilentlyContinue
}

$threadRoot = Join-Path $RepoRoot ".codex\threads\Sunset"
if (Test-Path $threadRoot) {
    $targets += Get-ChildItem $threadRoot -Recurse -File -Filter "memory_0.md" -ErrorAction SilentlyContinue
}

$specRoot = Join-Path $RepoRoot ".kiro\specs"
if (Test-Path $specRoot) {
    $targets += Get-ChildItem $specRoot -Recurse -File -ErrorAction SilentlyContinue | Where-Object {
        $_.Name -eq "memory.md" -or $_.Name -eq "001部分回执.md"
    }
}

$patterns = @(
    "127\.0\.0\.1:8080",
    "8080/mcp",
    "\bmcp-unity\b"
)

$hits = foreach ($file in ($targets | Sort-Object FullName -Unique)) {
    Select-String -Path $file.FullName -Pattern $patterns -ErrorAction SilentlyContinue
}

if (-not $hits) {
    Write-Output "legacy_mcp_reference_status: clean"
    exit 0
}

Write-Output "legacy_mcp_reference_status: found"
foreach ($hit in $hits) {
    "{0}:{1}: {2}" -f $hit.Path, $hit.LineNumber, $hit.Line.Trim()
}
