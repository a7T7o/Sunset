[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string]$ThreadName,

    [ValidateSet('task', 'governance')]
    [string]$Mode = 'task'
)

$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot 'StateCommon.ps1')

$repoRoot = Get-StateRepoRoot
$state = Read-ThreadState -ThreadName $ThreadName -RepoRoot $repoRoot
if ($null -eq $state) {
    throw "未找到线程状态：$ThreadName"
}

if ($state.status -notin @('ACTIVE', 'BLOCKED')) {
    throw "线程 $ThreadName 当前状态是 $($state.status)，不能进入 Ready-To-Sync。"
}

$readyLock = Acquire-StateExecutionLock -LockName 'ready-to-sync' -RepoRoot $repoRoot
try {
    if (@($state.shared_files).Count -gt 0) {
        $conflicts = Get-BClassConflicts -ThreadName $ThreadName -SharedFiles @($state.shared_files) -TouchedTouchpoints @($state.touched_touchpoints) -RepoRoot $repoRoot
        if ($conflicts.Count -gt 0) {
            $state.status = 'BLOCKED'
            $state.blockers = @($conflicts)
            $state.updated_at = Get-StateTimestamp
            Write-ThreadState -State $state -RepoRoot $repoRoot | Out-Null
            throw ($conflicts -join [Environment]::NewLine)
        }
    }

    if (@($state.a_class_locked_paths).Count -gt 0) {
        $checkScript = Join-Path $repoRoot '.kiro\scripts\locks\Check-Lock.ps1'
        foreach ($path in @($state.a_class_locked_paths)) {
            $result = & powershell -NoProfile -ExecutionPolicy Bypass -File $checkScript -TargetPath $path 2>&1
            if ($LASTEXITCODE -ne 0) {
                throw "A 类锁校验失败：$path`n$($result -join [Environment]::NewLine)"
            }

            $payload = ($result | Out-String) | ConvertFrom-Json
            if ($payload.state -ne 'locked' -or $payload.lock.owner_thread -ne $ThreadName) {
                throw "A 类锁已丢失或 owner 不匹配：$path"
            }
        }
    }

    $preflight = Invoke-StableGitSafeSync -Action 'preflight' -OwnerThread $ThreadName -Mode $Mode -IncludePaths @($state.expected_sync_paths) -RepoRoot $repoRoot
    if ($preflight.ExitCode -ne 0) {
        $state.status = 'BLOCKED'
        $state.blockers = @(Get-UsefulBlockersFromOutput -OutputLines $preflight.Output)
        $state.updated_at = Get-StateTimestamp
        Write-ThreadState -State $state -RepoRoot $repoRoot | Out-Null

        [PSCustomObject]@{
            thread    = $ThreadName
            status    = $state.status
            blockers  = $state.blockers
            preflight = $preflight.Output
        } | ConvertTo-Json -Depth 6
        exit 2
    }

    $state.status = 'READY_TO_SYNC'
    $state.blockers = @()
    $state.current_branch = Get-StateCurrentBranch -RepoRoot $repoRoot
    $state.updated_at = Get-StateTimestamp
    Write-ThreadState -State $state -RepoRoot $repoRoot | Out-Null

    [PSCustomObject]@{
        thread              = $ThreadName
        status              = $state.status
        expected_sync_paths = @($state.expected_sync_paths)
        next_action         = '可以继续执行 git-safe-sync.ps1 -Action sync；A 类锁保持到本轮真正收口为止。'
    } | ConvertTo-Json -Depth 5
}
finally {
    Release-StateExecutionLock -LockHandle $readyLock
}
