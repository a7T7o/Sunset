[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string]$ThreadName,

    [Parameter(Mandatory = $true)]
    [string]$CurrentSlice,

    [Parameter(Mandatory = $true)]
    [string[]]$TargetPaths,

    [string[]]$CarriedForeignPaths = @(),

    [string[]]$SharedTouchpoints = @(),

    [string[]]$ExpectedSyncPaths = @(),

    [switch]$ForceReplace
)

$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot 'StateCommon.ps1')

$repoRoot = Get-StateRepoRoot
Ensure-StateDirectories -RepoRoot $repoRoot

$existing = Read-ThreadState -ThreadName $ThreadName -RepoRoot $repoRoot
if ($null -ne $existing -and -not $ForceReplace) {
    if ($existing.status -in @('ACTIVE', 'READY_TO_SYNC')) {
        throw "线程 $ThreadName 当前仍处于 $($existing.status)；如需覆盖，请显式使用 -ForceReplace。"
    }
}

$normalizedTargets = Convert-ToNormalizedPathSet -Paths $TargetPaths -RepoRoot $repoRoot
$normalizedCarried = Convert-ToNormalizedPathSet -Paths $CarriedForeignPaths -RepoRoot $repoRoot
$normalizedTouchpoints = @($SharedTouchpoints | Where-Object { -not [string]::IsNullOrWhiteSpace($_) } | ForEach-Object { $_.Trim() } | Select-Object -Unique)

$ownedPaths = [System.Collections.Generic.List[string]]::new()
$sharedFiles = [System.Collections.Generic.List[string]]::new()
$aClassPaths = [System.Collections.Generic.List[string]]::new()

foreach ($path in $normalizedTargets) {
    $class = Get-PathClass -RelativePath $path
    switch ($class) {
        'A' {
            $ownedPaths.Add($path)
            $aClassPaths.Add($path)
        }
        'B' {
            $sharedFiles.Add($path)
        }
        default {
            $ownedPaths.Add($path)
        }
    }
}

if ($sharedFiles.Count -gt 0 -and $normalizedTouchpoints.Count -eq 0) {
    throw 'B 类共享文件必须显式声明 SharedTouchpoints。'
}

$bClassConflicts = Get-BClassConflicts -ThreadName $ThreadName -SharedFiles @($sharedFiles) -TouchedTouchpoints $normalizedTouchpoints -RepoRoot $repoRoot
if ($bClassConflicts.Count -gt 0) {
    throw ($bClassConflicts -join [Environment]::NewLine)
}

$acquireScript = Join-Path $repoRoot '.kiro\scripts\locks\Acquire-Lock.ps1'
if (-not (Test-Path -LiteralPath $acquireScript)) {
    throw "Acquire-Lock.ps1 不存在：$acquireScript"
}

$lockedPaths = [System.Collections.Generic.List[string]]::new()
try {
    foreach ($path in $aClassPaths) {
        & powershell -NoProfile -ExecutionPolicy Bypass -File $acquireScript `
            -TargetPath $path `
            -OwnerThread $ThreadName `
            -Task $CurrentSlice `
            -Checkpoint "Begin-Slice:$CurrentSlice" `
            -OwnerWorkdir $repoRoot | Out-Null

        if ($LASTEXITCODE -ne 0) {
            throw "A 类对象锁申请失败：$path"
        }

        $lockedPaths.Add($path)
    }
}
catch {
    if ($lockedPaths.Count -gt 0) {
        $cleanupState = [PSCustomObject]@{
            thread               = $ThreadName
            a_class_locked_paths = @($lockedPaths)
        }

        try {
            Release-AClassLocksForState -State $cleanupState -ReleaseNote 'begin-slice-rollback' -RepoRoot $repoRoot
        }
        catch {
        }
    }

    throw
}

$resolvedExpectedSyncPaths = if (@($ExpectedSyncPaths).Count -gt 0) {
    Convert-ToNormalizedPathSet -Paths $ExpectedSyncPaths -RepoRoot $repoRoot
}
else {
    Get-DefaultExpectedSyncPaths -OwnedPaths @($ownedPaths) -CarriedForeignPaths $normalizedCarried -SharedFiles @($sharedFiles) -RepoRoot $repoRoot
}

$state = [PSCustomObject]@{
    thread                = $ThreadName
    status                = 'ACTIVE'
    workdir               = $repoRoot
    current_branch        = (Get-StateCurrentBranch -RepoRoot $repoRoot)
    base_head             = (Get-StateHead -RepoRoot $repoRoot)
    current_slice         = $CurrentSlice.Trim()
    owned_paths           = @($ownedPaths)
    carried_foreign_paths = @($normalizedCarried)
    shared_files          = @($sharedFiles)
    touched_touchpoints   = @($normalizedTouchpoints)
    expected_sync_paths   = @($resolvedExpectedSyncPaths)
    a_class_locked_paths  = @($lockedPaths)
    blockers              = @()
    updated_at            = (Get-StateTimestamp)
}

$statePath = Write-ThreadState -State $state -RepoRoot $repoRoot

[PSCustomObject]@{
    state_path           = $statePath
    thread               = $state.thread
    status               = $state.status
    current_slice        = $state.current_slice
    owned_paths          = $state.owned_paths
    carried_foreign      = $state.carried_foreign_paths
    shared_files         = $state.shared_files
    touched_touchpoints  = $state.touched_touchpoints
    a_class_locked_paths = $state.a_class_locked_paths
    expected_sync_paths  = $state.expected_sync_paths
    note                 = 'ACTIVE 施工期默认不要写 tracked memory；只在 Ready-To-Sync 或 Park 时结算。'
} | ConvertTo-Json -Depth 6
