param(
    [string]$RepoRoot = "D:\Unity\Unity_learning\Sunset"
)

$ErrorActionPreference = "Stop"

$roots = @(
    (Join-Path $RepoRoot ".kiro"),
    (Join-Path $RepoRoot ".codex")
)

$excludeWildcards = @(
    "*\\state_backups\\*",
    "*\\文档归档\\*",
    "*\\History\\*",
    "*\\archive\\*",
    "*\\archived\\*",
    "*\\当前运行基线与开发规则\\memory_0.md",
    "*\\当前运行基线与开发规则\\memory_1.md",
    "*\\当前运行基线与开发规则\\memory_2.md",
    "*\\当前运行基线与开发规则\\memory_3.md",
    "*\\mcp-live-baseline.md"
)

$patterns = @(
    "127\.0\.0\.1:8080",
    "8080/mcp",
    "\bmcp-unity\b"
)

$files = Get-ChildItem $roots -Recurse -File -ErrorAction SilentlyContinue |
    Where-Object {
        $_.Extension -in ".md", ".txt", ".toml", ".yaml", ".yml"
    } |
    Where-Object {
        $full = $_.FullName.ToLowerInvariant()
        -not ($excludeWildcards | Where-Object { $full -like $_.ToLowerInvariant() })
    }

$hits = foreach ($file in $files) {
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
