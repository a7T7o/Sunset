[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string]$ThreadName,

    [string[]]$Blockers = @(),

    [string]$Reason = 'parked-by-operator'
)

$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot 'StateCommon.ps1')

$repoRoot = Get-StateRepoRoot
$state = Read-ThreadState -ThreadName $ThreadName -RepoRoot $repoRoot
if ($null -eq $state) {
    throw "未找到线程状态：$ThreadName"
}

if (@($state.a_class_locked_paths).Count -gt 0) {
    Release-AClassLocksForState -State $state -ReleaseNote $Reason -RepoRoot $repoRoot
}

$state.status = 'PARKED'
$state.a_class_locked_paths = @()
$rawBlockers = if (@($Blockers).Count -gt 0) {
    @($Blockers)
}
else {
    @($state.blockers)
}

$state.blockers = @(
    $rawBlockers |
        Where-Object { -not [string]::IsNullOrWhiteSpace($_) } |
        ForEach-Object { $_.ToString().Trim() }
)
$state.updated_at = Get-StateTimestamp
Write-ThreadState -State $state -RepoRoot $repoRoot | Out-Null

[PSCustomObject]@{
    thread   = $ThreadName
    status   = $state.status
    blockers = @($state.blockers)
    note     = '已合法停车；现在才允许把 blocker / 恢复点结算进 tracked memory。'
} | ConvertTo-Json -Depth 5
