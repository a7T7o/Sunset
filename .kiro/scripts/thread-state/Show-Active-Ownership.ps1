[CmdletBinding()]
param(
    [switch]$AsJson,
    [switch]$IncludeHandedOff
)

$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot 'StateCommon.ps1')

$repoRoot = Get-StateRepoRoot
$states = Get-AllThreadStates -RepoRoot $repoRoot

if (-not $IncludeHandedOff) {
    $states = @($states | Where-Object { $_.status -ne 'HANDED_OFF' })
}

$rows = @(
    $states |
        Sort-Object status, thread |
        ForEach-Object {
            [PSCustomObject]@{
                Thread       = $_.thread
                Status       = $_.status
                Slice        = $_.current_slice
                AClassLocked = (@($_.a_class_locked_paths) -join '; ')
                SharedFiles  = (@($_.shared_files) -join '; ')
                Touchpoints  = (@($_.touched_touchpoints) -join '; ')
                OwnedPaths   = (@($_.owned_paths) -join '; ')
                Blockers     = (@($_.blockers) -join '; ')
                UpdatedAt    = $_.updated_at
            }
        }
)

if ($AsJson) {
    ConvertTo-Json -InputObject @($rows) -Depth 5
    exit 0
}

if ($rows.Count -eq 0) {
    Write-Output '当前没有活跃线程状态。'
    exit 0
}

$rows | Format-Table -AutoSize
