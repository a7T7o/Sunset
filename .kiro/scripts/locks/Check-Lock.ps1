[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string]$TargetPath
)

$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot 'LockCommon.ps1')

$target = Resolve-LockTarget -TargetPath $TargetPath
$existingLock = Read-LockFile -LockPath $target.ActiveLockPath

if ($null -eq $existingLock) {
    [PSCustomObject]@{
        state       = 'unlocked'
        target_path = $target.TargetFullPath
        lock_path   = $target.ActiveLockPath
    } | ConvertTo-Json -Depth 5
    exit 0
}

[PSCustomObject]@{
    state       = 'locked'
    target_path = $target.TargetFullPath
    lock_path   = $target.ActiveLockPath
    lock        = $existingLock
} | ConvertTo-Json -Depth 8
exit 0
