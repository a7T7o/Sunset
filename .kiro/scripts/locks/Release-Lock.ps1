[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string]$TargetPath,

    [Parameter(Mandatory = $true)]
    [string]$OwnerThread,

    [string]$ReleasedBy,

    [string]$ReleaseNote = 'checkpoint complete; release lock'
)

$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot 'LockCommon.ps1')

$target = Resolve-LockTarget -TargetPath $TargetPath
$dirs = Ensure-LockDirectories -RepoRoot $target.RepoRoot

if (-not (Test-Path -LiteralPath $target.ActiveLockPath)) {
    throw "Active lock not found: $($target.ActiveLockPath)"
}

$existingLock = Read-LockFile -LockPath $target.ActiveLockPath
if ($null -eq $existingLock) {
    throw "Lock file is unreadable: $($target.ActiveLockPath)"
}

if ($existingLock.owner_thread -ne $OwnerThread) {
    throw "Current thread is not the lock owner. Owner: $($existingLock.owner_thread)"
}

if ([string]::IsNullOrWhiteSpace($ReleasedBy)) {
    $ReleasedBy = $OwnerThread
}

$releasedAt = Get-Date
$historyName = '{0}.{1}.released.json' -f [System.IO.Path]::GetFileNameWithoutExtension($target.LockFileName), $releasedAt.ToString('yyyyMMdd-HHmmss')
$historyPath = Join-Path $dirs.History $historyName

$releasedLock = [ordered]@{}
foreach ($property in $existingLock.PSObject.Properties) {
    $releasedLock[$property.Name] = $property.Value
}

$releasedLock['released_at'] = $releasedAt.ToString('o')
$releasedLock['released_by'] = $ReleasedBy
$releasedLock['release_note'] = $ReleaseNote
$releasedLock['final_branch'] = Get-GitText -RepoRoot $target.RepoRoot -Arguments @('branch', '--show-current')
$releasedLock['final_head'] = Get-GitText -RepoRoot $target.RepoRoot -Arguments @('rev-parse', 'HEAD')

$historyJson = Convert-LockToJson -LockObject $releasedLock
Write-Utf8NoBom -Path $historyPath -Content $historyJson
Remove-Item -LiteralPath $target.ActiveLockPath -Force

[PSCustomObject]@{
    state            = 'released'
    target_path      = $target.TargetFullPath
    active_lock_path = $target.ActiveLockPath
    history_path     = $historyPath
    released_lock    = $releasedLock
} | ConvertTo-Json -Depth 8
exit 0
