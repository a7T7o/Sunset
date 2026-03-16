[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string]$TargetPath,

    [Parameter(Mandatory = $true)]
    [string]$OwnerThread,

    [string]$OwnerGroup = 'Sunset',

    [string]$Task = 'unnamed-task',

    [string]$Checkpoint = 'checkpoint-not-specified',

    [string]$GrantedBy = 'global-governance',

    [int]$LeaseMinutes = 120,

    [string]$OwnerBranch,

    [string]$OwnerHead,

    [string]$OwnerWorkdir
)

$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot 'LockCommon.ps1')

$target = Resolve-LockTarget -TargetPath $TargetPath
$dirs = Ensure-LockDirectories -RepoRoot $target.RepoRoot

if ([string]::IsNullOrWhiteSpace($OwnerBranch)) {
    $OwnerBranch = Get-GitText -RepoRoot $target.RepoRoot -Arguments @('branch', '--show-current')
}

if ([string]::IsNullOrWhiteSpace($OwnerHead)) {
    $OwnerHead = Get-GitText -RepoRoot $target.RepoRoot -Arguments @('rev-parse', 'HEAD')
}

if ([string]::IsNullOrWhiteSpace($OwnerWorkdir)) {
    $OwnerWorkdir = $target.RepoRoot
}

$existingLock = Read-LockFile -LockPath $target.ActiveLockPath
if ($null -ne $existingLock) {
    [PSCustomObject]@{
        state         = 'locked'
        target_path   = $target.TargetFullPath
        lock_path     = $target.ActiveLockPath
        existing_lock = $existingLock
    } | ConvertTo-Json -Depth 8
    exit 2
}

$now = Get-Date
$lockObject = [ordered]@{
    lock_version        = 1
    lock_type           = 'A-class-hotfile'
    target_path         = $target.TargetFullPath
    target_relative     = $target.RelativePath
    target_key          = $target.Key
    owner_thread        = $OwnerThread
    owner_group         = $OwnerGroup
    owner_branch        = $OwnerBranch
    owner_head          = $OwnerHead
    owner_workdir       = $OwnerWorkdir
    task                = $Task
    checkpoint          = $Checkpoint
    granted_by          = $GrantedBy
    created_at          = $now.ToString('o')
    heartbeat_at        = $now.ToString('o')
    lease_minutes       = $LeaseMinutes
    expected_release_at = $now.AddMinutes($LeaseMinutes).ToString('o')
    release_condition   = 'Release with Release-Lock.ps1 after the approved checkpoint is complete'
}

$lockJson = Convert-LockToJson -LockObject $lockObject
$bytes = [System.Text.UTF8Encoding]::new($false).GetBytes($lockJson)
$stream = [System.IO.File]::Open($target.ActiveLockPath, [System.IO.FileMode]::CreateNew, [System.IO.FileAccess]::Write, [System.IO.FileShare]::None)

try {
    $stream.Write($bytes, 0, $bytes.Length)
}
finally {
    $stream.Dispose()
}

[PSCustomObject]@{
    state       = 'acquired'
    target_path = $target.TargetFullPath
    lock_path   = $target.ActiveLockPath
    lock        = $lockObject
} | ConvertTo-Json -Depth 8
exit 0
