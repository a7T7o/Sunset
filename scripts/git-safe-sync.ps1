param(
    [Parameter(Mandatory = $true)]
    [ValidateSet('preflight', 'ensure-branch', 'grant-branch', 'request-branch', 'cancel-branch-request', 'requeue-branch-request', 'wake-next', 'return-main', 'sync')]
    [string]$Action,

    [Parameter(Mandatory = $true)]
    [string]$OwnerThread,

    [ValidateSet('governance', 'task')]
    [string]$Mode = 'task',

    [string]$BranchName,

    [string[]]$ScopeRoots = @(),

    [string[]]$IncludePaths = @(),

    [string]$CheckpointHint,

    [string]$QueueNote
)

$ErrorActionPreference = 'Stop'

function Invoke-Git {
    param(
        [Parameter(Mandatory = $true)]
        [string[]]$Arguments,
        [switch]$AllowFailure
    )

    $stdoutPath = [System.IO.Path]::GetTempFileName()
    $stderrPath = [System.IO.Path]::GetTempFileName()
    $quotedArguments = @()

    foreach ($argument in $Arguments) {
        if ($argument -match '[\s"]') {
            $escaped = $argument.Replace('"', '\"')
            $quotedArguments += '"' + $escaped + '"'
        }
        else {
            $quotedArguments += $argument
        }
    }

    try {
        $process = Start-Process -FilePath 'git' -ArgumentList ($quotedArguments -join ' ') -NoNewWindow -Wait -PassThru -RedirectStandardOutput $stdoutPath -RedirectStandardError $stderrPath
        $output = @()

        if (Test-Path $stdoutPath) {
            $stdoutLines = Get-Content $stdoutPath -Encoding UTF8
            if ($null -ne $stdoutLines) {
                $output += $stdoutLines
            }
        }

        if (Test-Path $stderrPath) {
            $stderrLines = Get-Content $stderrPath -Encoding UTF8 | Where-Object { -not [string]::IsNullOrWhiteSpace($_) }
            if ($null -ne $stderrLines) {
                $output += $stderrLines
            }
        }

        if (-not $AllowFailure -and $process.ExitCode -ne 0) {
            throw "git $($Arguments -join ' ') 失败：$($output -join [Environment]::NewLine)"
        }

        [PSCustomObject]@{
            ExitCode = $process.ExitCode
            Output   = @($output)
        }
    }
    finally {
        Remove-Item $stdoutPath, $stderrPath -ErrorAction SilentlyContinue
    }
}

function Normalize-StatusPath {
    param([string]$RawPath)

    $path = $RawPath.Trim()

    if ($path.StartsWith('"') -and $path.EndsWith('"')) {
        $path = $path.Substring(1, $path.Length - 2)
        $path = $path.Replace('\"', '"')
        $path = $path.Replace('\\', '\')
    }

    if ($path -match ' -> ') {
        $path = ($path -split ' -> ', 2)[1]
    }

    $path = $path.Replace('\', '/')
    return $path
}

function Normalize-InputPath {
    param([string]$Path)

    $normalized = $Path.Replace('\', '/').Trim()
    if ($normalized.StartsWith('./')) {
        $normalized = $normalized.Substring(2)
    }
    return $normalized.TrimEnd('/')
}

function Expand-PathList {
    param([string[]]$Paths)

    $expanded = @()

    foreach ($item in @($Paths)) {
        if ([string]::IsNullOrWhiteSpace($item)) {
            continue
        }

        $parts = $item -split '[;,]'
        foreach ($part in $parts) {
            if (-not [string]::IsNullOrWhiteSpace($part)) {
                $expanded += $part.Trim()
            }
        }
    }

    return @($expanded)
}

function Get-RepoRoot {
    return (Invoke-Git -Arguments @('rev-parse', '--show-toplevel')).Output[0].Trim()
}

function Get-CurrentBranch {
    return (Invoke-Git -Arguments @('branch', '--show-current')).Output[0].Trim()
}

function Convert-MarkdownScalarValue {
    param([string]$Value)

    $normalized = $Value.Trim()

    if ($normalized.StartsWith('`') -and $normalized.EndsWith('`')) {
        $normalized = $normalized.Substring(1, $normalized.Length - 2)
    }

    if (($normalized.StartsWith("'") -and $normalized.EndsWith("'")) -or ($normalized.StartsWith('"') -and $normalized.EndsWith('"'))) {
        $normalized = $normalized.Substring(1, $normalized.Length - 2)
    }

    return $normalized.Trim()
}

function Get-SharedRootOccupancyPath {
    return (Join-Path (Get-RepoRoot) '.kiro/locks/shared-root-branch-occupancy.md')
}

function Get-OccupancyDateStamp {
    return (Get-Date).ToString('yyyy-MM-dd')
}

function Get-OccupancyTimestamp {
    return (Get-Date).ToString('yyyy-MM-dd HH:mm:ss zzz')
}

function Format-MarkdownScalarLine {
    param(
        [string]$Key,
        [string]$Value
    )

    $normalized = if ([string]::IsNullOrWhiteSpace($Value)) { 'none' } else { $Value.Trim() }
    return "- $($Key): ``$normalized``"
}

function Set-SharedRootOccupancyValues {
    param([hashtable]$Values)

    $path = Get-SharedRootOccupancyPath
    if (-not (Test-Path -LiteralPath $path)) {
        throw "FATAL: shared root 占用文档不存在：$path"
    }

    $lines = [System.Collections.Generic.List[string]]::new()
    foreach ($line in (Get-Content -LiteralPath $path -Encoding UTF8)) {
        $lines.Add($line)
    }

    $handled = @{}
    for ($i = 0; $i -lt $lines.Count; $i++) {
        if ($lines[$i] -match '^- ([a-z_]+): (.+)$') {
            $key = $Matches[1]
            if ($Values.ContainsKey($key)) {
                $lines[$i] = Format-MarkdownScalarLine -Key $key -Value ([string]$Values[$key])
                $handled[$key] = $true
            }
        }
    }

    $insertIndex = $lines.IndexOf('## 解释口径')
    if ($insertIndex -lt 0) {
        $insertIndex = $lines.Count
    }

    foreach ($entry in $Values.GetEnumerator()) {
        if (-not $handled.ContainsKey($entry.Key)) {
            $lines.Insert($insertIndex, (Format-MarkdownScalarLine -Key $entry.Key -Value ([string]$entry.Value)))
            $insertIndex++
        }
    }

    Set-Content -LiteralPath $path -Encoding UTF8 -Value $lines
}

function Get-SharedRootOccupancyRecord {
    $path = Get-SharedRootOccupancyPath

    if (-not (Test-Path -LiteralPath $path)) {
        return $null
    }

    $values = @{}
    foreach ($line in (Get-Content -LiteralPath $path -Encoding UTF8)) {
        if ($line -match '^- ([a-z_]+): (.+)$') {
            $values[$Matches[1]] = Convert-MarkdownScalarValue -Value $Matches[2]
        }
    }

    if ($values.Count -eq 0) {
        return $null
    }

    $isNeutral = $false
    if ($values.ContainsKey('is_neutral')) {
        $isNeutral = $values['is_neutral'].Trim().ToLowerInvariant() -eq 'true'
    }

    return [PSCustomObject]@{
        OccupancyPath      = $path
        RootPath           = $values['root_path']
        OwnerMode          = $values['owner_mode']
        OwnerThread        = $values['owner_thread']
        CurrentBranch      = $values['current_branch']
        LastVerifiedHead   = $values['last_verified_head']
        IsNeutral          = $isNeutral
        BlockingDirtyScope = $values['blocking_dirty_scope']
        DailyPolicy        = $values['daily_policy']
        WorktreePolicy     = $values['worktree_policy']
        LeaseState         = $values['lease_state']
        BranchGrantState   = $values['branch_grant_state']
        BranchGrantOwner   = $values['branch_grant_owner_thread']
        BranchGrantBranch  = $values['branch_grant_branch']
        BranchGrantUpdated = $values['branch_grant_updated']
        LastUpdated        = $values['last_updated']
    }
}

function Test-IsSharedRootWorkspace {
    param(
        [string]$RepoRoot,
        $OccupancyRecord
    )

    if ($null -eq $OccupancyRecord -or [string]::IsNullOrWhiteSpace($OccupancyRecord.RootPath)) {
        return $false
    }

    $normalizedRepoRoot = (Normalize-InputPath -Path $RepoRoot).ToLowerInvariant()
    $normalizedSharedRoot = (Normalize-InputPath -Path $OccupancyRecord.RootPath).ToLowerInvariant()
    return $normalizedRepoRoot -eq $normalizedSharedRoot
}

function Assert-SharedRootBranchGrant {
    param(
        $Occupancy,
        [string]$OwnerThread,
        [string]$TargetBranch
    )

    if ($null -eq $Occupancy) {
        throw 'FATAL: shared root 缺少 occupancy 文档，禁止 ensure-branch。'
    }

    if (-not $Occupancy.IsNeutral -or $Occupancy.CurrentBranch -ne 'main') {
        throw "FATAL: shared root 当前不是 neutral main 大厅（current_branch='$($Occupancy.CurrentBranch)'，is_neutral='$($Occupancy.IsNeutral)'），禁止 ensure-branch。"
    }

    if ($Occupancy.BranchGrantState -ne 'granted') {
        throw "FATAL: OwnerThread '$OwnerThread' 未拿到 shared root 的独占分支租约，禁止 ensure-branch。"
    }

    if ($Occupancy.BranchGrantOwner -ne $OwnerThread) {
        throw "FATAL: shared root 当前租约属于 '$($Occupancy.BranchGrantOwner)'，而不是 '$OwnerThread'，禁止 ensure-branch。"
    }

    if ($Occupancy.BranchGrantBranch -ne $TargetBranch) {
        throw "FATAL: shared root 当前租约只允许切到 '$($Occupancy.BranchGrantBranch)'，你请求的是 '$TargetBranch'，禁止 ensure-branch。"
    }
}

function Set-SharedRootTaskActive {
    param(
        [string]$OwnerThread,
        [string]$TargetBranch
    )

    Set-SharedRootOccupancyValues -Values ([ordered]@{
            owner_mode                = 'task-branch-active'
            owner_thread              = $OwnerThread
            current_branch            = $TargetBranch
            last_verified_head        = (Get-HeadShort)
            is_neutral                = 'false'
            lease_state               = 'task-active'
            branch_grant_state        = 'consumed'
            branch_grant_owner_thread = 'none'
            branch_grant_branch       = 'none'
            branch_grant_updated      = (Get-OccupancyTimestamp)
            blocking_dirty_scope      = 'owned-task-branch'
            last_updated              = (Get-OccupancyDateStamp)
        })

    $queueEntry = Set-SharedRootQueueTaskActive -OwnerThread $OwnerThread -TargetBranch $TargetBranch
    $checkpointHint = 'none'
    $note = 'none'
    $ticket = $null

    if ($null -ne $queueEntry) {
        $checkpointHint = $queueEntry.CheckpointHint
        $note = $queueEntry.Note
        $ticket = $queueEntry.Ticket
    }

    Set-SharedRootActiveSession -OwnerThread $OwnerThread -TargetBranch $TargetBranch -CheckpointHint $checkpointHint -Note $note -Ticket $ticket | Out-Null
}

function Restore-SharedRootOccupancyToHead {
    $relativePath = Get-SharedRootOccupancyRelativePath
    Invoke-Git -Arguments @('restore', '--source=HEAD', '--', $relativePath) | Out-Null
}

function Set-SharedRootNeutralState {
    Restore-SharedRootOccupancyToHead
}

function Get-SharedRootQueueSpecPath {
    return (Join-Path (Get-RepoRoot) '.kiro/locks/shared-root-queue.md')
}

function Get-SharedRootQueueRuntimePath {
    return (Join-Path (Get-RepoRoot) '.kiro/locks/active/shared-root-queue.lock.json')
}

function Get-SharedRootActiveSessionRuntimePath {
    return (Join-Path (Get-RepoRoot) '.kiro/locks/active/shared-root-active-session.lock.json')
}

function Get-SharedRootActiveSessionHistoryPath {
    return (Join-Path (Get-RepoRoot) '.kiro/locks/history/shared-root-active-session.history.json')
}

function Set-OrAddPsNoteProperty {
    param(
        $Object,
        [string]$Name,
        $Value
    )

    if ($null -eq $Object) {
        return $false
    }

    $property = $Object.PSObject.Properties[$Name]
    if ($null -eq $property) {
        $Object | Add-Member -NotePropertyName $Name -NotePropertyValue $Value
        return $true
    }

    $currentValue = $property.Value
    if (($null -eq $currentValue -and $null -ne $Value) -or
        ($null -ne $currentValue -and $null -eq $Value) -or
        ([string]$currentValue -ne [string]$Value)) {
        $property.Value = $Value
        return $true
    }

    return $false
}

function Repair-SharedRootActiveSessionRecord {
    param($Session)

    if ($null -eq $Session) {
        return [PSCustomObject]@{
            Session = $null
            Changed = $false
        }
    }

    $timestamp = Get-OccupancyTimestamp
    $enteredAt = if ([string]::IsNullOrWhiteSpace([string]$Session.entered_at)) {
        if ([string]::IsNullOrWhiteSpace([string]$Session.last_updated)) { $timestamp } else { [string]$Session.last_updated }
    }
    else {
        [string]$Session.entered_at
    }

    $lastUpdated = if ([string]::IsNullOrWhiteSpace([string]$Session.last_updated)) {
        $enteredAt
    }
    else {
        [string]$Session.last_updated
    }

    # 兼容旧 runtime：缺字段时在读路径补齐，避免尾部 touch/complete 因属性不存在而再炸。
    $normalized = [ordered]@{
        version                  = if ($null -eq $Session.version -or [string]::IsNullOrWhiteSpace([string]$Session.version)) { 1 } else { [int]$Session.version }
        owner_thread             = if ([string]::IsNullOrWhiteSpace([string]$Session.owner_thread)) { 'unknown' } else { [string]$Session.owner_thread }
        target_branch            = if ([string]::IsNullOrWhiteSpace([string]$Session.target_branch)) { 'unknown' } else { [string]$Session.target_branch }
        ticket                   = if ($null -eq $Session.ticket -or [string]::IsNullOrWhiteSpace([string]$Session.ticket)) { $null } else { [int]$Session.ticket }
        checkpoint_hint          = if ([string]::IsNullOrWhiteSpace([string]$Session.checkpoint_hint)) { 'none' } else { [string]$Session.checkpoint_hint }
        note                     = if ([string]::IsNullOrWhiteSpace([string]$Session.note)) { 'none' } else { [string]$Session.note }
        entered_at               = $enteredAt
        last_updated             = $lastUpdated
        hold_category            = if ([string]::IsNullOrWhiteSpace([string]$Session.hold_category)) { 'unknown' } else { [string]$Session.hold_category }
        recommended_hold_minutes = if ($null -eq $Session.recommended_hold_minutes -or [string]::IsNullOrWhiteSpace([string]$Session.recommended_hold_minutes)) { $null } else { [int]$Session.recommended_hold_minutes }
        hold_summary             = if ([string]::IsNullOrWhiteSpace([string]$Session.hold_summary)) { 'none' } else { [string]$Session.hold_summary }
        source                   = if ([string]::IsNullOrWhiteSpace([string]$Session.source)) { 'legacy-runtime' } else { [string]$Session.source }
        last_reason              = if ([string]::IsNullOrWhiteSpace([string]$Session.last_reason)) { 'none' } else { [string]$Session.last_reason }
    }

    $changed = $false
    foreach ($entry in $normalized.GetEnumerator()) {
        if (Set-OrAddPsNoteProperty -Object $Session -Name $entry.Key -Value $entry.Value) {
            $changed = $true
        }
    }

    return [PSCustomObject]@{
        Session = $Session
        Changed = $changed
    }
}

function Get-SharedRootDraftsRootPath {
    return (Join-Path (Get-RepoRoot) '.codex/drafts')
}

function Get-SharedRootDraftsGuidePath {
    return (Join-Path (Get-SharedRootDraftsRootPath) 'README.md')
}

function Convert-ToSafePathLeaf {
    param([string]$Value)

    $result = if ([string]::IsNullOrWhiteSpace($Value)) { 'unknown' } else { $Value.Trim() }

    foreach ($char in [System.IO.Path]::GetInvalidFileNameChars()) {
        $result = $result.Replace([string]$char, '-')
    }

    $result = $result.Replace('/', '-').Replace('\', '-').Replace(':', '-')
    $result = [regex]::Replace($result, '\s+', '-')
    $result = [regex]::Replace($result, '-{2,}', '-').Trim('-')

    if ([string]::IsNullOrWhiteSpace($result)) {
        return 'unknown'
    }

    return $result
}

function Get-SharedRootOwnerDraftsPath {
    param([string]$OwnerThread)

    $ownerLeaf = Convert-ToSafePathLeaf -Value $OwnerThread
    return (Join-Path (Get-SharedRootDraftsRootPath) $ownerLeaf)
}

function Get-SharedRootDraftFileHint {
    param(
        [string]$OwnerThread,
        [string]$TargetBranch
    )

    $ownerDir = Get-SharedRootOwnerDraftsPath -OwnerThread $OwnerThread
    $branchLeaf = Convert-ToSafePathLeaf -Value $TargetBranch
    $timestamp = (Get-Date).ToString('yyyyMMdd-HHmmss')
    return (Join-Path $ownerDir "$timestamp-$branchLeaf.md")
}

function Write-SharedRootDraftHints {
    param(
        [string]$OwnerThread,
        [string]$TargetBranch
    )

    Write-Output "DRAFTS_ROOT: $(Get-SharedRootDraftsRootPath)"
    Write-Output "DRAFTS_GUIDE_PATH: $(Get-SharedRootDraftsGuidePath)"
    Write-Output "DRAFTS_OWNER_DIR: $(Get-SharedRootOwnerDraftsPath -OwnerThread $OwnerThread)"
    Write-Output "DRAFT_FILE_HINT: $(Get-SharedRootDraftFileHint -OwnerThread $OwnerThread -TargetBranch $TargetBranch)"
}

function Get-SharedRootHoldWindowPolicy {
    param(
        [string]$CheckpointHint,
        [string]$Note,
        [string]$TargetBranch
    )

    $normalized = @($CheckpointHint, $Note, $TargetBranch) -join ' '
    $normalized = $normalized.ToLowerInvariant()

    if ($normalized -match 'docs|doc|audit|review|retest|requirements|design|tasks|memory') {
        return [PSCustomObject]@{
            Category   = 'docs-fast-lane'
            MaxMinutes = 3
            Summary    = '文档 / 审计 / docs-first 事务应快进快出，建议 3 分钟内完成。'
        }
    }

    if ($normalized -match 'unity|scene|prefab|inspector|play|mcp|animation|vfx') {
        return [PSCustomObject]@{
            Category   = 'unity-guarded-window'
            MaxMinutes = 15
            Summary    = 'Unity / MCP 事务允许更长窗口，但必须聚焦单次验证；超过 15 分钟应拆批。'
        }
    }

    return [PSCustomObject]@{
        Category   = 'code-checkpoint'
        MaxMinutes = 8
        Summary    = '默认代码 checkpoint 窗口 8 分钟；超过应拆小或先 return-main。'
    }
}

function Get-SharedRootActiveSession {
    $path = Get-SharedRootActiveSessionRuntimePath

    if (-not (Test-Path -LiteralPath $path)) {
        return $null
    }

    $rawState = Get-Content -Raw -LiteralPath $path -Encoding UTF8
    if ([string]::IsNullOrWhiteSpace($rawState)) {
        return $null
    }

    try {
        $state = $rawState | ConvertFrom-Json
    }
    catch {
        throw "FATAL: shared root active session runtime 解析失败：$path。请先核查 JSON 是否损坏。"
    }

    $repair = Repair-SharedRootActiveSessionRecord -Session $state
    if ($repair.Changed) {
        $json = $repair.Session | ConvertTo-Json -Depth 6
        Set-Content -LiteralPath $path -Encoding UTF8 -Value $json
    }

    return $repair.Session
}

function Set-SharedRootActiveSession {
    param(
        [string]$OwnerThread,
        [string]$TargetBranch,
        [string]$CheckpointHint,
        [string]$Note,
        $Ticket
    )

    $path = Get-SharedRootActiveSessionRuntimePath
    $directory = Split-Path -Parent $path
    if (-not (Test-Path -LiteralPath $directory)) {
        New-Item -ItemType Directory -Path $directory -Force | Out-Null
    }

    $policy = Get-SharedRootHoldWindowPolicy -CheckpointHint $CheckpointHint -Note $Note -TargetBranch $TargetBranch
    $timestamp = Get-OccupancyTimestamp

    $state = [ordered]@{
        version                  = 1
        owner_thread             = $OwnerThread
        target_branch            = $TargetBranch
        ticket                   = if ($null -eq $Ticket) { $null } else { [int]$Ticket }
        checkpoint_hint          = if ([string]::IsNullOrWhiteSpace($CheckpointHint)) { 'none' } else { $CheckpointHint }
        note                     = if ([string]::IsNullOrWhiteSpace($Note)) { 'none' } else { $Note }
        entered_at               = $timestamp
        last_updated             = $timestamp
        hold_category            = $policy.Category
        recommended_hold_minutes = [int]$policy.MaxMinutes
        hold_summary             = $policy.Summary
        source                   = 'ensure-branch'
        last_reason              = 'entered-task-active'
    }

    $json = $state | ConvertTo-Json -Depth 6
    Set-Content -LiteralPath $path -Encoding UTF8 -Value $json

    return [PSCustomObject]$state
}

function Touch-SharedRootActiveSession {
    param(
        [string]$OwnerThread,
        [string]$TargetBranch,
        [string]$Reason
    )

    $active = Get-SharedRootActiveSession
    if ($null -eq $active) {
        return $null
    }

    if ($active.owner_thread -ne $OwnerThread -or $active.target_branch -ne $TargetBranch) {
        return $null
    }

    $path = Get-SharedRootActiveSessionRuntimePath
    Set-OrAddPsNoteProperty -Object $active -Name 'last_updated' -Value (Get-OccupancyTimestamp) | Out-Null
    if (-not [string]::IsNullOrWhiteSpace($Reason)) {
        Set-OrAddPsNoteProperty -Object $active -Name 'last_reason' -Value $Reason | Out-Null
    }

    $json = $active | ConvertTo-Json -Depth 6
    Set-Content -LiteralPath $path -Encoding UTF8 -Value $json
    return $active
}

function Add-SharedRootActiveSessionHistory {
    param($HistoryEntry)

    $path = Get-SharedRootActiveSessionHistoryPath
    $directory = Split-Path -Parent $path
    if (-not (Test-Path -LiteralPath $directory)) {
        New-Item -ItemType Directory -Path $directory -Force | Out-Null
    }

    $entries = @()
    if (Test-Path -LiteralPath $path) {
        $rawState = Get-Content -Raw -LiteralPath $path -Encoding UTF8
        if (-not [string]::IsNullOrWhiteSpace($rawState)) {
            try {
                $history = $rawState | ConvertFrom-Json
                $entries = @($history.entries)
            }
            catch {
                throw "FATAL: shared root active session history 解析失败：$path。请先核查 JSON 是否损坏。"
            }
        }
    }

    $entries += $HistoryEntry
    if ($entries.Count -gt 50) {
        $entries = @($entries | Select-Object -Last 50)
    }

    $state = [ordered]@{
        version      = 1
        last_updated = Get-OccupancyTimestamp
        entries      = @($entries)
    }

    $json = $state | ConvertTo-Json -Depth 8
    Set-Content -LiteralPath $path -Encoding UTF8 -Value $json
}

function Complete-SharedRootActiveSession {
    param(
        [string]$OwnerThread,
        [string]$TargetBranch,
        [string]$CompletionStatus
    )

    $active = Get-SharedRootActiveSession
    if ($null -eq $active) {
        return $null
    }

    if ($active.owner_thread -ne $OwnerThread -or $active.target_branch -ne $TargetBranch) {
        return $null
    }

    $closedAt = Get-OccupancyTimestamp
    $actualHoldMinutes = $null
    try {
        $enteredAt = [DateTimeOffset]::Parse([string]$active.entered_at)
        $closedAtDto = [DateTimeOffset]::Parse($closedAt)
        $actualHoldMinutes = [math]::Round(($closedAtDto - $enteredAt).TotalMinutes, 2)
    }
    catch {
        $actualHoldMinutes = $null
    }

    $budgetStatus = 'unknown'
    if ($null -ne $actualHoldMinutes -and $null -ne $active.recommended_hold_minutes) {
        if ([double]$actualHoldMinutes -le [double]$active.recommended_hold_minutes) {
            $budgetStatus = 'within-budget'
        }
        else {
            $budgetStatus = 'overtime'
        }
    }

    $historyEntry = [ordered]@{
        owner_thread             = [string]$active.owner_thread
        target_branch            = [string]$active.target_branch
        ticket                   = $active.ticket
        checkpoint_hint          = [string]$active.checkpoint_hint
        note                     = [string]$active.note
        entered_at               = [string]$active.entered_at
        last_updated             = [string]$active.last_updated
        returned_at              = $closedAt
        hold_category            = [string]$active.hold_category
        recommended_hold_minutes = $active.recommended_hold_minutes
        hold_summary             = [string]$active.hold_summary
        source                   = [string]$active.source
        last_reason              = [string]$active.last_reason
        actual_hold_minutes      = $actualHoldMinutes
        budget_status            = $budgetStatus
        completion_status        = if ([string]::IsNullOrWhiteSpace($CompletionStatus)) { 'returned-main' } else { $CompletionStatus }
    }

    Add-SharedRootActiveSessionHistory -HistoryEntry $historyEntry
    Remove-Item (Get-SharedRootActiveSessionRuntimePath) -ErrorAction SilentlyContinue

    return [PSCustomObject]$historyEntry
}

function New-SharedRootQueueRecord {
    param(
        [object[]]$Entries = @(),
        [int]$NextTicket = 1,
        $CurrentServingTicket = $null
    )

    return [PSCustomObject]@{
        RuntimePath           = Get-SharedRootQueueRuntimePath
        SpecPath              = Get-SharedRootQueueSpecPath
        QueueEnabled          = $true
        Policy                = 'request-branch-yield-then-grant'
        NextTicket            = $NextTicket
        CurrentServingTicket  = $CurrentServingTicket
        LastUpdated           = Get-OccupancyTimestamp
        Entries               = @($Entries)
    }
}

function Repair-SharedRootQueueRecord {
    param(
        $Queue,
        $Occupancy
    )

    if ($null -eq $Queue) {
        return [PSCustomObject]@{
            Queue   = $Queue
            Changed = $false
        }
    }

    $changed = $false
    $timestamp = Get-OccupancyTimestamp

    foreach ($entry in @($Queue.Entries)) {
        if ($null -eq $entry) {
            continue
        }

        if ($entry.State -eq 'waiting' -and $null -ne $Occupancy -and $Occupancy.CurrentBranch -eq 'main' -and $Occupancy.BranchGrantState -eq 'granted' -and $Occupancy.BranchGrantOwner -eq $entry.OwnerThread -and $Occupancy.BranchGrantBranch -eq $entry.TargetBranch) {
            $entry.State = 'granted'
            $entry.LastSeen = $timestamp
            if ($entry.GrantedAt -eq 'none' -or [string]::IsNullOrWhiteSpace($entry.GrantedAt)) {
                $entry.GrantedAt = $timestamp
            }
            $entry.LastReason = 'reconciled-grant-from-occupancy'
            $changed = $true
            continue
        }

        if ($entry.State -eq 'granted') {
            $isLiveGrant = $null -ne $Occupancy -and $Occupancy.CurrentBranch -eq 'main' -and $Occupancy.BranchGrantState -eq 'granted' -and $Occupancy.BranchGrantOwner -eq $entry.OwnerThread -and $Occupancy.BranchGrantBranch -eq $entry.TargetBranch
            $isLiveTask = $null -ne $Occupancy -and -not $Occupancy.IsNeutral -and $Occupancy.OwnerThread -eq $entry.OwnerThread -and $Occupancy.CurrentBranch -eq $entry.TargetBranch

            if ($isLiveTask) {
                $entry.State = 'task-active'
                $entry.LastSeen = $timestamp
                if ($entry.GrantedAt -eq 'none' -or [string]::IsNullOrWhiteSpace($entry.GrantedAt)) {
                    $entry.GrantedAt = $timestamp
                }
                $entry.LastReason = 'reconciled-task-active-from-occupancy'
                $changed = $true
            }
            elseif (-not $isLiveGrant) {
                $entry.State = 'cancelled'
                $entry.LastSeen = $timestamp
                $entry.LastReason = 'reconciled-missing-grant'
                $changed = $true
            }
            continue
        }

        if ($entry.State -eq 'task-active') {
            $isLiveTask = $null -ne $Occupancy -and -not $Occupancy.IsNeutral -and $Occupancy.OwnerThread -eq $entry.OwnerThread -and $Occupancy.CurrentBranch -eq $entry.TargetBranch
            if (-not $isLiveTask) {
                $entry.State = 'completed'
                $entry.LastSeen = $timestamp
                if ($entry.CompletedAt -eq 'none' -or [string]::IsNullOrWhiteSpace($entry.CompletedAt)) {
                    $entry.CompletedAt = $timestamp
                }
                $entry.LastReason = 'reconciled-missing-task-active'
                $changed = $true
            }
        }
    }

    $expectedServing = $null
    $taskActiveEntries = @($Queue.Entries | Where-Object { $_.State -eq 'task-active' } | Sort-Object Ticket | Select-Object -First 1)
    if ($taskActiveEntries.Count -gt 0) {
        $expectedServing = [int]$taskActiveEntries[0].Ticket
    }
    else {
        $grantedEntries = @($Queue.Entries | Where-Object { $_.State -eq 'granted' } | Sort-Object Ticket | Select-Object -First 1)
        if ($grantedEntries.Count -gt 0) {
            $expectedServing = [int]$grantedEntries[0].Ticket
        }
    }

    $currentServing = $Queue.CurrentServingTicket
    if (($null -eq $currentServing -and $null -ne $expectedServing) -or ($null -ne $currentServing -and $null -eq $expectedServing) -or ($null -ne $currentServing -and $null -ne $expectedServing -and [int]$currentServing -ne [int]$expectedServing)) {
        $Queue.CurrentServingTicket = $expectedServing
        $changed = $true
    }

    return [PSCustomObject]@{
        Queue   = $Queue
        Changed = $changed
    }
}

function Get-SharedRootQueueRecord {
    $path = Get-SharedRootQueueRuntimePath

    if (-not (Test-Path -LiteralPath $path)) {
        return (New-SharedRootQueueRecord)
    }

    $rawState = Get-Content -Raw -LiteralPath $path -Encoding UTF8
    if ([string]::IsNullOrWhiteSpace($rawState)) {
        return (New-SharedRootQueueRecord)
    }

    try {
        $state = $rawState | ConvertFrom-Json
    }
    catch {
        throw "FATAL: shared root queue runtime 解析失败：$path。请先核查 JSON 是否损坏，再决定是否清理该 runtime 文件。"
    }
    $entries = @()

    foreach ($entry in @($state.entries)) {
        if ($null -eq $entry) {
            continue
        }

        $entries += [PSCustomObject]@{
            Ticket         = [int]$entry.ticket
            OwnerThread    = [string]$entry.owner_thread
            TargetBranch   = [string]$entry.target_branch
            State          = [string]$entry.state
            RequestedAt    = [string]$entry.requested_at
            LastSeen       = [string]$entry.last_seen
            CheckpointHint = [string]$entry.checkpoint_hint
            Note           = [string]$entry.note
            GrantedAt      = [string]$entry.granted_at
            CompletedAt    = [string]$entry.completed_at
            LastReason     = [string]$entry.last_reason
        }
    }

    $currentServing = $null
    if ($null -ne $state.current_serving_ticket -and -not [string]::IsNullOrWhiteSpace([string]$state.current_serving_ticket)) {
        $currentServing = [int]$state.current_serving_ticket
    }

    $queue = [PSCustomObject]@{
        RuntimePath           = $path
        SpecPath              = Get-SharedRootQueueSpecPath
        QueueEnabled          = if ($null -eq $state.queue_enabled) { $true } else { [bool]$state.queue_enabled }
        Policy                = if ([string]::IsNullOrWhiteSpace([string]$state.policy)) { 'request-branch-yield-then-grant' } else { [string]$state.policy }
        NextTicket            = if ($null -eq $state.next_ticket) { 1 } else { [int]$state.next_ticket }
        CurrentServingTicket  = $currentServing
        LastUpdated           = [string]$state.last_updated
        Entries               = @($entries)
    }

    $repair = Repair-SharedRootQueueRecord -Queue $queue -Occupancy (Get-SharedRootOccupancyRecord)
    if ($repair.Changed) {
        Set-SharedRootQueueRecord -Queue $repair.Queue
    }

    return $repair.Queue
}

function Set-SharedRootQueueRecord {
    param($Queue)

    $path = Get-SharedRootQueueRuntimePath
    $directory = Split-Path -Parent $path
    if (-not (Test-Path -LiteralPath $directory)) {
        New-Item -ItemType Directory -Path $directory -Force | Out-Null
    }

    $entries = @()
    foreach ($entry in @($Queue.Entries | Sort-Object Ticket)) {
        $entries += [ordered]@{
            ticket          = [int]$entry.Ticket
            owner_thread    = if ([string]::IsNullOrWhiteSpace($entry.OwnerThread)) { 'none' } else { [string]$entry.OwnerThread }
            target_branch   = if ([string]::IsNullOrWhiteSpace($entry.TargetBranch)) { 'none' } else { [string]$entry.TargetBranch }
            state           = if ([string]::IsNullOrWhiteSpace($entry.State)) { 'waiting' } else { [string]$entry.State }
            requested_at    = if ([string]::IsNullOrWhiteSpace($entry.RequestedAt)) { 'none' } else { [string]$entry.RequestedAt }
            last_seen       = if ([string]::IsNullOrWhiteSpace($entry.LastSeen)) { 'none' } else { [string]$entry.LastSeen }
            checkpoint_hint = if ([string]::IsNullOrWhiteSpace($entry.CheckpointHint)) { 'none' } else { [string]$entry.CheckpointHint }
            note            = if ([string]::IsNullOrWhiteSpace($entry.Note)) { 'none' } else { [string]$entry.Note }
            granted_at      = if ([string]::IsNullOrWhiteSpace($entry.GrantedAt)) { 'none' } else { [string]$entry.GrantedAt }
            completed_at    = if ([string]::IsNullOrWhiteSpace($entry.CompletedAt)) { 'none' } else { [string]$entry.CompletedAt }
            last_reason     = if ([string]::IsNullOrWhiteSpace($entry.LastReason)) { 'none' } else { [string]$entry.LastReason }
        }
    }

    $state = [ordered]@{
        version                = 1
        queue_enabled          = if ($null -eq $Queue.QueueEnabled) { $true } else { [bool]$Queue.QueueEnabled }
        policy                 = if ([string]::IsNullOrWhiteSpace($Queue.Policy)) { 'request-branch-yield-then-grant' } else { [string]$Queue.Policy }
        next_ticket            = if ($null -eq $Queue.NextTicket) { 1 } else { [int]$Queue.NextTicket }
        current_serving_ticket = if ($null -eq $Queue.CurrentServingTicket) { $null } else { [int]$Queue.CurrentServingTicket }
        last_updated           = Get-OccupancyTimestamp
        entries                = @($entries)
    }

    $json = $state | ConvertTo-Json -Depth 10
    Set-Content -LiteralPath $path -Encoding UTF8 -Value $json
}

function Get-OpenSharedRootQueueEntry {
    param(
        $Queue,
        [string]$OwnerThread,
        [string]$TargetBranch
    )

    $openStates = @('waiting', 'granted', 'task-active')
    $matches = @(
        $Queue.Entries |
            Where-Object {
                $_.OwnerThread -eq $OwnerThread -and
                $_.TargetBranch -eq $TargetBranch -and
                ($openStates -contains $_.State)
            } |
            Sort-Object Ticket -Descending
    )

    if ($matches.Count -eq 0) {
        return $null
    }

    return $matches[0]
}

function Get-SharedRootWaitingEntries {
    param($Queue)

    return @(
        $Queue.Entries |
            Where-Object { $_.State -eq 'waiting' } |
            Sort-Object Ticket
    )
}

function Get-SharedRootNextWaitingEntry {
    param($Queue)

    $waitingEntries = @(Get-SharedRootWaitingEntries -Queue $Queue | Select-Object -First 1)
    if ($waitingEntries.Count -eq 0) {
        return $null
    }

    return $waitingEntries[0]
}

function Upsert-SharedRootQueueEntry {
    param(
        $Queue,
        [string]$OwnerThread,
        [string]$TargetBranch,
        [string]$NewState,
        [string]$CheckpointHint,
        [string]$Note,
        [string]$LastReason
    )

    $timestamp = Get-OccupancyTimestamp
    $entry = Get-OpenSharedRootQueueEntry -Queue $Queue -OwnerThread $OwnerThread -TargetBranch $TargetBranch

    if ($null -eq $entry) {
        $entry = [PSCustomObject]@{
            Ticket         = [int]$Queue.NextTicket
            OwnerThread    = $OwnerThread
            TargetBranch   = $TargetBranch
            State          = if ([string]::IsNullOrWhiteSpace($NewState)) { 'waiting' } else { $NewState }
            RequestedAt    = $timestamp
            LastSeen       = $timestamp
            CheckpointHint = if ([string]::IsNullOrWhiteSpace($CheckpointHint)) { 'none' } else { $CheckpointHint }
            Note           = if ([string]::IsNullOrWhiteSpace($Note)) { 'none' } else { $Note }
            GrantedAt      = 'none'
            CompletedAt    = 'none'
            LastReason     = if ([string]::IsNullOrWhiteSpace($LastReason)) { 'none' } else { $LastReason }
        }
        $Queue.Entries += $entry
        $Queue.NextTicket = [int]$Queue.NextTicket + 1
    }
    else {
        if (-not [string]::IsNullOrWhiteSpace($NewState)) {
            $entry.State = $NewState
        }
        $entry.LastSeen = $timestamp
        if (-not [string]::IsNullOrWhiteSpace($CheckpointHint)) {
            $entry.CheckpointHint = $CheckpointHint
        }
        if (-not [string]::IsNullOrWhiteSpace($Note)) {
            $entry.Note = $Note
        }
        if (-not [string]::IsNullOrWhiteSpace($LastReason)) {
            $entry.LastReason = $LastReason
        }
    }

    if ($entry.State -eq 'granted' -and ($entry.GrantedAt -eq 'none' -or [string]::IsNullOrWhiteSpace($entry.GrantedAt))) {
        $entry.GrantedAt = $timestamp
    }

    if ($entry.State -eq 'task-active' -and ($entry.GrantedAt -eq 'none' -or [string]::IsNullOrWhiteSpace($entry.GrantedAt))) {
        $entry.GrantedAt = $timestamp
    }

    if ($entry.State -eq 'completed') {
        $entry.CompletedAt = $timestamp
    }

    return $entry
}

function Set-SharedRootQueueTaskActive {
    param(
        [string]$OwnerThread,
        [string]$TargetBranch
    )

    $queue = Get-SharedRootQueueRecord
    $entry = Get-OpenSharedRootQueueEntry -Queue $queue -OwnerThread $OwnerThread -TargetBranch $TargetBranch
    if ($null -eq $entry) {
        return $null
    }

    $entry = Upsert-SharedRootQueueEntry -Queue $queue -OwnerThread $OwnerThread -TargetBranch $TargetBranch -NewState 'task-active' -CheckpointHint $entry.CheckpointHint -Note $entry.Note -LastReason 'task-active'
    $queue.CurrentServingTicket = [int]$entry.Ticket
    Set-SharedRootQueueRecord -Queue $queue
    return $entry
}

function Complete-SharedRootQueueEntry {
    param(
        [string]$OwnerThread,
        [string]$TargetBranch
    )

    $queue = Get-SharedRootQueueRecord
    $entry = Get-OpenSharedRootQueueEntry -Queue $queue -OwnerThread $OwnerThread -TargetBranch $TargetBranch
    if ($null -eq $entry) {
        return
    }

    $entry = Upsert-SharedRootQueueEntry -Queue $queue -OwnerThread $OwnerThread -TargetBranch $TargetBranch -NewState 'completed' -CheckpointHint $entry.CheckpointHint -Note $entry.Note -LastReason 'return-main'
    if ($queue.CurrentServingTicket -eq [int]$entry.Ticket) {
        $queue.CurrentServingTicket = $null
    }
    Set-SharedRootQueueRecord -Queue $queue
}

function Cancel-SharedRootOpenBranchRequest {
    param(
        [string]$OwnerThread,
        [string]$TargetBranch,
        [string]$Note
    )

    $repoRoot = Get-RepoRoot
    $occupancy = Get-SharedRootOccupancyRecord
    if (-not (Test-IsSharedRootWorkspace -RepoRoot $repoRoot -OccupancyRecord $occupancy)) {
        throw 'FATAL: 只允许在 shared root 上取消排队或租约请求。'
    }

    Assert-TaskBranchMatchesOwner -Branch $TargetBranch -OwnerThread $OwnerThread

    $branch = Get-CurrentBranch
    $queue = Get-SharedRootQueueRecord
    $entry = Get-OpenSharedRootQueueEntry -Queue $queue -OwnerThread $OwnerThread -TargetBranch $TargetBranch
    if ($null -eq $entry) {
        return [PSCustomObject]@{
            Found         = $false
            CancelledEntry = $null
            ReleasedGrant = $false
            NextWaiting   = Get-SharedRootNextWaitingEntry -Queue $queue
        }
    }

    if ($entry.State -eq 'task-active') {
        throw "FATAL: '$OwnerThread' 当前请求已进入 task-active。请使用 return-main 归还 shared root，而不是 cancel-branch-request。"
    }

    if ($branch -ne 'main') {
        throw "FATAL: 当前 live 分支是 '$branch'；只有 shared root 的 main 大厅才能取消 waiting / granted 请求。"
    }

    $releasedGrant = $false
    if ($entry.State -eq 'granted' -and $null -ne $occupancy -and $occupancy.BranchGrantState -eq 'granted') {
        if ($occupancy.BranchGrantOwner -ne $OwnerThread -or $occupancy.BranchGrantBranch -ne $TargetBranch) {
            throw "FATAL: occupancy 当前登记的 grant 属于 '$($occupancy.BranchGrantOwner)' -> '$($occupancy.BranchGrantBranch)'，与你的取消请求 '$OwnerThread' -> '$TargetBranch' 不符。"
        }

        Set-SharedRootNeutralState
        $releasedGrant = $true
    }

    $reason = if ([string]::IsNullOrWhiteSpace($Note)) { 'cancelled-by-owner' } else { $Note }
    $entry = Upsert-SharedRootQueueEntry -Queue $queue -OwnerThread $OwnerThread -TargetBranch $TargetBranch -NewState 'cancelled' -CheckpointHint $entry.CheckpointHint -Note $entry.Note -LastReason $reason
    if ($queue.CurrentServingTicket -eq [int]$entry.Ticket) {
        $queue.CurrentServingTicket = $null
    }
    Set-SharedRootQueueRecord -Queue $queue

    $queue = Get-SharedRootQueueRecord
    return [PSCustomObject]@{
        Found          = $true
        CancelledEntry = $entry
        ReleasedGrant  = $releasedGrant
        NextWaiting    = Get-SharedRootNextWaitingEntry -Queue $queue
        Queue          = $queue
    }
}

function Cancel-SharedRootBranchRequest {
    param(
        [string]$OwnerThread,
        [string]$TargetBranch,
        [string]$Note
    )

    $result = Cancel-SharedRootOpenBranchRequest -OwnerThread $OwnerThread -TargetBranch $TargetBranch -Note $Note

    if (-not $result.Found) {
        Write-Output 'STATUS: NO_OPEN_REQUEST'
        Write-Output "OWNER_THREAD: $OwnerThread"
        Write-Output "BRANCH: $TargetBranch"
        Write-Output 'MESSAGE: 当前没有可取消的 waiting / granted 请求。'
        return
    }

    Write-Output 'STATUS: CANCELLED'
    Write-Output "OWNER_THREAD: $OwnerThread"
    Write-Output "BRANCH: $TargetBranch"
    Write-Output "CANCELLED_TICKET: $($result.CancelledEntry.Ticket)"
    Write-Output "RELEASED_GRANT: $(if ($result.ReleasedGrant) { 'true' } else { 'false' })"
    if ($null -ne $result.NextWaiting) {
        Write-Output "NEXT_IN_LINE_TICKET: $($result.NextWaiting.Ticket)"
        Write-Output "NEXT_IN_LINE_OWNER_THREAD: $($result.NextWaiting.OwnerThread)"
        Write-Output "NEXT_IN_LINE_BRANCH: $($result.NextWaiting.TargetBranch)"
        Write-Output 'NEXT_ACTION: 治理线程可执行 wake-next，或向队首线程发放唤醒 Prompt。'
    }
    else {
        Write-Output 'NEXT_IN_LINE_TICKET: none'
        Write-Output 'NEXT_ACTION: 当前队列已空，无需继续唤醒。'
    }
}

function Requeue-SharedRootBranchRequest {
    param(
        [string]$OwnerThread,
        [string]$TargetBranch,
        [string]$CheckpointHint,
        [string]$Note
    )

    $cancelNote = 'requeue-old-ticket'
    if (-not [string]::IsNullOrWhiteSpace($Note)) {
        $cancelNote = "requeue-old-ticket: $Note"
    }

    $result = Cancel-SharedRootOpenBranchRequest -OwnerThread $OwnerThread -TargetBranch $TargetBranch -Note $cancelNote

    if (-not $result.Found) {
        Write-Output 'STATUS: NO_OPEN_REQUEST'
        Write-Output "OWNER_THREAD: $OwnerThread"
        Write-Output "BRANCH: $TargetBranch"
        Write-Output 'MESSAGE: 当前没有可重排的 open request；如需首次入队，请改用 request-branch。'
        return
    }

    Write-Output "REQUEUE_PREVIOUS_TICKET: $($result.CancelledEntry.Ticket)"
    Request-SharedRootBranchLease -OwnerThread $OwnerThread -TargetBranch $TargetBranch -CheckpointHint $CheckpointHint -Note $Note
}

function Get-SharedRootWakeGate {
    $repoRoot = Get-RepoRoot
    $branch = Get-CurrentBranch
    $occupancy = Get-SharedRootOccupancyRecord

    if (-not (Test-IsSharedRootWorkspace -RepoRoot $repoRoot -OccupancyRecord $occupancy)) {
        throw 'FATAL: 只允许在 shared root 上执行 wake-next。'
    }

    if ($branch -ne 'main') {
        return [PSCustomObject]@{
            CanWake   = $false
            Code      = 'BLOCKED'
            Reason    = "当前 live 分支是 '$branch'，只有 main 大厅才能执行 wake-next。"
            Occupancy = $occupancy
        }
    }

    $entries = @(Get-BlockingStatusEntries -Entries (Get-StatusEntries) -Branch $branch -Occupancy $occupancy)
    if ($entries.Count -gt 0) {
        return [PSCustomObject]@{
            CanWake   = $false
            Code      = 'BLOCKED'
            Reason    = 'shared root 当前不干净，暂时不能执行 wake-next。'
            Occupancy = $occupancy
        }
    }

    if ($null -eq $occupancy) {
        throw 'FATAL: shared root 缺少 occupancy 文档，禁止 wake-next。'
    }

    if (-not $occupancy.IsNeutral -or $occupancy.CurrentBranch -ne 'main') {
        return [PSCustomObject]@{
            CanWake   = $false
            Code      = 'BLOCKED'
            Reason    = "shared root 当前不是 neutral main 状态（current_branch='$($occupancy.CurrentBranch)'，is_neutral='$($occupancy.IsNeutral)'）。"
            Occupancy = $occupancy
        }
    }

    if ($occupancy.BranchGrantState -eq 'granted' -and $occupancy.BranchGrantOwner -ne 'none') {
        return [PSCustomObject]@{
            CanWake   = $false
            Code      = 'BLOCKED'
            Reason    = "shared root 已存在未消费的 grant（owner='$($occupancy.BranchGrantOwner)'，branch='$($occupancy.BranchGrantBranch)'）。"
            Occupancy = $occupancy
        }
    }

    return [PSCustomObject]@{
        CanWake   = $true
        Code      = 'READY'
        Reason    = 'shared root 当前允许唤醒队首线程。'
        Occupancy = $occupancy
    }
}

function Wake-NextSharedRootBranchRequest {
    param([string]$DispatcherOwnerThread)

    $gate = Get-SharedRootWakeGate
    $queue = Get-SharedRootQueueRecord
    $nextWaiting = Get-SharedRootNextWaitingEntry -Queue $queue

    if (-not $gate.CanWake) {
        Write-Output 'STATUS: WAKE_BLOCKED'
        Write-Output "DISPATCHER_OWNER_THREAD: $DispatcherOwnerThread"
        Write-Output "REASON: $($gate.Reason)"
        if ($null -ne $nextWaiting) {
            Write-Output "QUEUE_HEAD_TICKET: $($nextWaiting.Ticket)"
            Write-Output "QUEUE_HEAD_OWNER_THREAD: $($nextWaiting.OwnerThread)"
            Write-Output "QUEUE_HEAD_BRANCH: $($nextWaiting.TargetBranch)"
        }
        return
    }

    if ($null -eq $nextWaiting) {
        Write-Output 'STATUS: NO_WAITING_REQUESTS'
        Write-Output "DISPATCHER_OWNER_THREAD: $DispatcherOwnerThread"
        Write-Output 'MESSAGE: 当前队列没有 waiting 条目，无需唤醒。'
        return
    }

    Grant-SharedRootBranchLease -OwnerThread $nextWaiting.OwnerThread -TargetBranch $nextWaiting.TargetBranch

    $queue = Get-SharedRootQueueRecord
    $entry = Get-OpenSharedRootQueueEntry -Queue $queue -OwnerThread $nextWaiting.OwnerThread -TargetBranch $nextWaiting.TargetBranch
    $policy = Get-SharedRootHoldWindowPolicy -CheckpointHint $entry.CheckpointHint -Note $entry.Note -TargetBranch $entry.TargetBranch
    Write-Output 'STATUS: WOKE_NEXT'
    Write-Output "DISPATCHER_OWNER_THREAD: $DispatcherOwnerThread"
    Write-Output "NEXT_IN_LINE_TICKET: $($entry.Ticket)"
    Write-Output "NEXT_IN_LINE_OWNER_THREAD: $($entry.OwnerThread)"
    Write-Output "NEXT_IN_LINE_BRANCH: $($entry.TargetBranch)"
    Write-Output "NEXT_IN_LINE_CHECKPOINT_HINT: $($entry.CheckpointHint)"
    Write-Output "NEXT_IN_LINE_NOTE: $($entry.Note)"
    Write-Output "NEXT_IN_LINE_RECOMMENDED_HOLD_MINUTES: $($policy.MaxMinutes)"
    Write-Output "NEXT_IN_LINE_HOLD_CATEGORY: $($policy.Category)"
    Write-SharedRootDraftHints -OwnerThread $entry.OwnerThread -TargetBranch $entry.TargetBranch
    Write-Output 'NEXT_ACTION: 通知该线程重做 live preflight；随后执行 request-branch（应返回 ALREADY_GRANTED）并继续 ensure-branch。'
}

function Get-OwnerBranchKeywords {
    param([string]$OwnerThread)

    $normalizedOwner = $OwnerThread.Trim().ToLowerInvariant()

    switch -Regex ($normalizedOwner) {
        'npc' { return @('npc') }
        'farm|农田' { return @('farm') }
        'spring-day1|spring|day1' { return @('spring-day1', 'spring', 'day1') }
        '导航|navigation|nav' { return @('navigation', 'nav') }
        '遮挡|occlusion' { return @('occlusion', '遮挡') }
        '项目文档总览|documentation|docs|about' { return @('documentation', 'docs', 'about') }
        'codex规则落地|governance|rules|rule|治理' { return @('governance', 'rules', 'rule', 'guard') }
        default {
            $asciiFallback = ($normalizedOwner -replace '[^a-z0-9/-]+', '')
            if (-not [string]::IsNullOrWhiteSpace($asciiFallback)) {
                return @($asciiFallback)
            }

            return @($normalizedOwner)
        }
    }
}

function Test-BranchOwnedByThread {
    param(
        [string]$Branch,
        [string]$OwnerThread
    )

    if ([string]::IsNullOrWhiteSpace($Branch) -or [string]::IsNullOrWhiteSpace($OwnerThread)) {
        return $false
    }

    $normalizedBranch = $Branch.Trim().ToLowerInvariant()
    $keywords = @(Get-OwnerBranchKeywords -OwnerThread $OwnerThread | Where-Object { -not [string]::IsNullOrWhiteSpace($_) })

    foreach ($keyword in $keywords) {
        if ($normalizedBranch.Contains($keyword.ToLowerInvariant())) {
            return $true
        }
    }

    return $false
}

function Get-BranchOwnershipMessage {
    param(
        [string]$Branch,
        [string]$OwnerThread
    )

    $keywords = @(Get-OwnerBranchKeywords -OwnerThread $OwnerThread | Where-Object { -not [string]::IsNullOrWhiteSpace($_) })
    return "FATAL: 当前所在分支 '$Branch' 与 OwnerThread '$OwnerThread' 不符；允许的分支语义关键词至少应包含：$($keywords -join ', ')。"
}

function Assert-TaskBranchMatchesOwner {
    param(
        [string]$Branch,
        [string]$OwnerThread
    )

    if (-not (Test-BranchOwnedByThread -Branch $Branch -OwnerThread $OwnerThread)) {
        throw (Get-BranchOwnershipMessage -Branch $Branch -OwnerThread $OwnerThread)
    }
}

function Get-HeadShort {
    return (Invoke-Git -Arguments @('rev-parse', '--short', 'HEAD')).Output[0].Trim()
}

function Get-CodeGuardProjectPath {
    return (Join-Path (Get-RepoRoot) 'scripts/CodexCodeGuard/CodexCodeGuard.csproj')
}

function Get-CodeGuardDllPath {
    $projectPath = Get-CodeGuardProjectPath
    $projectDirectory = Split-Path -Parent $projectPath
    return (Join-Path $projectDirectory 'bin/Release/net9.0/CodexCodeGuard.dll')
}

function Test-CodeGuardBuildRequired {
    $dllPath = Get-CodeGuardDllPath
    if (-not (Test-Path -LiteralPath $dllPath)) {
        return $true
    }

    $projectDirectory = Split-Path -Parent (Get-CodeGuardProjectPath)
    $latestInput = Get-ChildItem -Path $projectDirectory -Recurse -File |
        Where-Object { $_.Extension -in @('.cs', '.csproj') } |
        Sort-Object LastWriteTime -Descending |
        Select-Object -First 1

    if ($null -eq $latestInput) {
        return $false
    }

    return $latestInput.LastWriteTime -gt (Get-Item -LiteralPath $dllPath).LastWriteTime
}

function Ensure-CodeGuardBuilt {
    $projectPath = Get-CodeGuardProjectPath
    if (-not (Test-Path -LiteralPath $projectPath)) {
        throw "FATAL: CodexCodeGuard 项目不存在：$projectPath"
    }

    if (-not (Test-CodeGuardBuildRequired)) {
        return
    }

    $output = & dotnet build $projectPath -c Release --nologo 2>&1
    $exitCode = $LASTEXITCODE
    if ($exitCode -ne 0) {
        throw "FATAL: CodexCodeGuard 构建失败：$($output -join [Environment]::NewLine)"
    }
}

function New-EmptyCodeGuardReport {
    param([string]$Reason)

    return [PSCustomObject]@{
        Applies          = $false
        CanContinue      = $true
        Phase            = 'pre-sync'
        RepoRoot         = Get-RepoRoot
        OwnerThread      = 'none'
        Branch           = Get-CurrentBranch
        ChangedCodeFiles = @()
        AffectedAssemblies = @()
        ChecksRun        = @()
        Diagnostics      = @()
        Summary          = $Reason
        Reason           = $Reason
    }
}

function Invoke-CodeGuard {
    param(
        [string]$RepoRoot,
        [string]$OwnerThread,
        [string]$Branch,
        [object[]]$AllowedEntries,
        [string]$Phase
    )

    $candidatePaths = @(
        $AllowedEntries |
        Where-Object { $null -ne $_ -and -not [string]::IsNullOrWhiteSpace($_.Path) -and $_.Path.ToLowerInvariant().EndsWith('.cs') } |
        ForEach-Object {
            $normalized = Normalize-InputPath -Path $_.Path
            Join-Path $RepoRoot ($normalized.Replace('/', [System.IO.Path]::DirectorySeparatorChar))
        } |
        Sort-Object -Unique
    )

    if ($candidatePaths.Count -eq 0) {
        return (New-EmptyCodeGuardReport -Reason '本轮白名单未触发 C# 代码闸门。')
    }

    Ensure-CodeGuardBuilt
    $dllPath = Get-CodeGuardDllPath

    $arguments = @($dllPath, '--repo-root', $RepoRoot, '--phase', $Phase, '--owner-thread', $OwnerThread, '--branch', $Branch)
    foreach ($path in $candidatePaths) {
        $arguments += @('--path', $path)
    }

    $output = & dotnet @arguments 2>&1
    $exitCode = $LASTEXITCODE
    $lines = @($output | ForEach-Object { "$_" } | Where-Object { -not [string]::IsNullOrWhiteSpace($_) })
    $jsonLine = $lines | Select-Object -Last 1

    if ([string]::IsNullOrWhiteSpace($jsonLine)) {
        throw "FATAL: CodexCodeGuard 未返回 JSON 结果。原始输出：$($lines -join [Environment]::NewLine)"
    }

    try {
        $report = $jsonLine | ConvertFrom-Json
    }
    catch {
        throw "FATAL: CodexCodeGuard JSON 解析失败：$jsonLine"
    }

    if ($exitCode -ne 0 -and ($null -eq $report -or $report.CanContinue)) {
        throw "FATAL: CodexCodeGuard 进程失败但未返回明确阻断结果：$($lines -join [Environment]::NewLine)"
    }

    return $report
}

function Get-UpstreamCounts {
    $upstream = Invoke-Git -Arguments @('rev-parse', '--abbrev-ref', '--symbolic-full-name', '@{upstream}') -AllowFailure

    if ($upstream.ExitCode -eq 0 -and $upstream.Output.Count -gt 0 -and -not [string]::IsNullOrWhiteSpace($upstream.Output[0])) {
        $counts = (Invoke-Git -Arguments @('rev-list', '--left-right', '--count', '@{upstream}...HEAD')).Output[0].Trim()
        $parts = $counts -split '\s+'
        return [PSCustomObject]@{
            Label  = '@{upstream}...HEAD'
            Behind = [int]$parts[0]
            Ahead  = [int]$parts[1]
        }
    }

    $fallback = (Invoke-Git -Arguments @('rev-list', '--left-right', '--count', 'origin/main...main')).Output[0].Trim()
    $fallbackParts = $fallback -split '\s+'
    return [PSCustomObject]@{
        Label  = 'origin/main...main'
        Behind = [int]$fallbackParts[0]
        Ahead  = [int]$fallbackParts[1]
    }
}

function Get-StatusEntries {
    $result = Invoke-Git -Arguments @('-c', 'core.quotepath=false', 'status', '--porcelain', '-uall')
    $entries = @()

    foreach ($line in $result.Output) {
        if ([string]::IsNullOrWhiteSpace($line)) {
            continue
        }

        $entries += [PSCustomObject]@{
            Status = $line.Substring(0, 2)
            Path   = Normalize-StatusPath -RawPath $line.Substring(3)
        }
    }

    return @($entries)
}

function Test-PathMatch {
    param(
        [string]$Path,
        [string]$Rule
    )

    $normalizedPath = Normalize-InputPath -Path $Path
    $normalizedRule = Normalize-InputPath -Path $Rule

    if ($normalizedPath -eq $normalizedRule) {
        return $true
    }

    return $normalizedPath.StartsWith("$normalizedRule/")
}

function Get-GovernanceRules {
    return @(
        '.gitattributes',
        '.gitignore',
        'AGENTS.md',
        'scripts',
        '.kiro/steering',
        '.kiro/hooks'
    )
}

function Get-NoiseRules {
    return @(
        '.claude/settings.local.json',
        '.claude/worktrees'
    )
}

function Test-NoisePath {
    param([string]$Path)

    foreach ($rule in Get-NoiseRules) {
        if (Test-PathMatch -Path $Path -Rule $rule) {
            return $true
        }
    }

    return $false
}

function Get-SharedRootOccupancyRelativePath {
    return '.kiro/locks/shared-root-branch-occupancy.md'
}

function Test-SharedRootLeaseRuntimeDirtyPath {
    param(
        [string]$Path,
        [string]$Branch,
        $Occupancy
    )

    if ($null -eq $Occupancy) {
        return $false
    }

    $normalizedPath = Normalize-InputPath -Path $Path
    $occupancyPath = Normalize-InputPath -Path (Get-SharedRootOccupancyRelativePath)

    if ($normalizedPath -ne $occupancyPath) {
        return $false
    }

    if ($Branch -eq 'main') {
        return ($Occupancy.IsNeutral -and $Occupancy.CurrentBranch -eq 'main' -and $Occupancy.BranchGrantState -eq 'granted')
    }

    return ((-not $Occupancy.IsNeutral) -and $Occupancy.CurrentBranch -eq $Branch)
}

function Get-BlockingStatusEntries {
    param(
        [object[]]$Entries,
        [string]$Branch,
        $Occupancy
    )

    if ($null -eq $Entries -or @($Entries).Count -eq 0) {
        return @()
    }

    $filtered = @(
        $Entries | Where-Object {
            $null -ne $_ -and
            -not [string]::IsNullOrWhiteSpace($_.Path) -and
            -not (Test-NoisePath -Path $_.Path) -and
            -not (Test-SharedRootLeaseRuntimeDirtyPath -Path $_.Path -Branch $Branch -Occupancy $Occupancy)
        }
    )

    return @($filtered)
}

function Test-AllowedPath {
    param(
        [string]$Path,
        [string]$Mode,
        [string[]]$ScopeRoots,
        [string[]]$IncludePaths
    )

    $rules = @()
    $normalizedScopeRoots = Expand-PathList -Paths $ScopeRoots
    $normalizedIncludePaths = Expand-PathList -Paths $IncludePaths

    if ($Mode -eq 'governance') {
        $rules += Get-GovernanceRules
    }

    if ($null -ne $normalizedScopeRoots) {
        $rules += ($normalizedScopeRoots | Where-Object { -not [string]::IsNullOrWhiteSpace($_) })
    }

    if ($null -ne $normalizedIncludePaths) {
        $rules += ($normalizedIncludePaths | Where-Object { -not [string]::IsNullOrWhiteSpace($_) })
    }

    foreach ($rule in $rules) {
        if (Test-PathMatch -Path $Path -Rule $rule) {
            return $true
        }
    }

    return $false
}

function Get-PathGroup {
    param([string]$Path)

    if (Test-NoisePath -Path $Path) { return '已知本地噪音' }
    if ($Path -like 'Assets/*' -or $Path -like 'Packages/*' -or $Path -like 'ProjectSettings/*') { return '实现/资源改动' }
    if ($Path -like '.kiro/specs/Steering规则区优化/*' -or $Path -like '.kiro/specs/000_代办/*' -or $Path -like '.kiro/steering/*' -or $Path -like '.kiro/hooks/*' -or $Path -eq '.gitattributes' -or $Path -eq '.gitignore' -or $Path -eq 'AGENTS.md' -or $Path -like 'scripts/*') { return '治理线改动' }
    if ($Path -like '.kiro/specs/农田系统/*') { return '农田线改动' }
    if ($Path -like '.kiro/specs/900_开篇/*') { return '开篇线改动' }
    if ($Path -like '.kiro/about/*') { return 'about治理线改动' }
    if ($Path -like '.codex/threads/*') { return '线程记忆改动' }
    return '其他改动'
}

function Test-GovernanceReportPath {
    param([string]$Path)

    if ([string]::IsNullOrWhiteSpace($Path)) {
        return $false
    }

    foreach ($rule in Get-GovernanceRules) {
        if (Test-PathMatch -Path $Path -Rule $rule) {
            return $true
        }
    }

    return (
        (Test-PathMatch -Path $Path -Rule '.codex/threads') -or
        (Test-PathMatch -Path $Path -Rule '.kiro/specs/Codex规则落地') -or
        (Test-PathMatch -Path $Path -Rule '.kiro/specs/共享根执行模型与吞吐重构') -or
        (Test-PathMatch -Path $Path -Rule '.kiro/about')
    )
}

function Get-ThreadMemoryOwnerHint {
    param([string]$Path)

    if ([string]::IsNullOrWhiteSpace($Path)) {
        return $null
    }

    $segments = @(($Path -replace '\\', '/').Split('/', [System.StringSplitOptions]::RemoveEmptyEntries))
    if ($segments.Count -ge 5 -and $segments[0] -ieq '.codex' -and $segments[1] -ieq 'threads') {
        return $segments[3]
    }

    if ($segments.Count -ge 3 -and $segments[0] -ieq '.codex' -and $segments[1] -ieq 'threads') {
        return 'thread-governance'
    }

    return $null
}

function Test-DirtyHardBlockPath {
    param([string]$Path)

    $normalizedPath = Normalize-InputPath -Path $Path
    if ([string]::IsNullOrWhiteSpace($normalizedPath)) {
        return $false
    }

    $comparisonPath = $normalizedPath.ToLowerInvariant()

    if ($comparisonPath -eq 'assets/yyy_scripts/controller/input/gameinputmanager.cs') {
        return $true
    }

    if ($comparisonPath.StartsWith('projectsettings/') -or $comparisonPath.StartsWith('packages/')) {
        return $true
    }

    if ($comparisonPath.EndsWith('.unity') -or $comparisonPath.EndsWith('.unity.meta')) {
        return $true
    }

    if ($comparisonPath.StartsWith('assets/222_prefabs/')) {
        return $true
    }

    if ($comparisonPath.StartsWith('assets/111_data/')) {
        return $true
    }

    if ($comparisonPath.StartsWith('assets/100_anim/')) {
        return $true
    }

    return $comparisonPath.StartsWith('assets/sprites/')
}

function Get-DirtyLevel {
    param(
        [string]$Path,
        [string]$Category,
        [string]$Branch,
        [string]$Mode
    )

    if ($Category -eq '已知本地噪音' -or $Category -eq 'shared root 租约运行态脏改') {
        return 'D0'
    }

    if (Test-DirtyHardBlockPath -Path $Path) {
        return 'D3'
    }

    if (Test-GovernanceReportPath -Path $Path) {
        return 'D1'
    }

    if (-not [string]::IsNullOrWhiteSpace($Branch) -and $Branch.ToLowerInvariant().StartsWith('codex/')) {
        return 'D2'
    }

    return 'D3'
}

function Get-DirtyOwnerHint {
    param(
        [string]$Path,
        [string]$Category,
        [string]$Branch,
        [string]$Mode,
        [string]$OwnerThread
    )

    $normalizedPath = Normalize-InputPath -Path $Path
    $comparisonPath = if ([string]::IsNullOrWhiteSpace($normalizedPath)) { '' } else { $normalizedPath.ToLowerInvariant() }

    if ($Category -eq '已知本地噪音') { return 'noise' }
    if ($Category -eq 'shared root 租约运行态脏改') { return 'shared-root-runtime' }
    if ($comparisonPath -eq 'assets/yyy_scripts/controller/input/gameinputmanager.cs') { return 'hotfile-owner-required' }
    if ($comparisonPath.EndsWith('/primary.unity')) { return 'scene-hotfile-owner-required' }
    $threadMemoryOwnerHint = Get-ThreadMemoryOwnerHint -Path $Path
    if (-not [string]::IsNullOrWhiteSpace($threadMemoryOwnerHint)) { return $threadMemoryOwnerHint }
    if (Test-GovernanceReportPath -Path $Path) { return 'Codex规则落地' }
    if ((Test-PathMatch -Path $Path -Rule '.kiro/specs/农田系统') -or $comparisonPath -match '(^|/)farm(/|$)' -or $comparisonPath -match 'hotbar') { return '农田交互修复V2' }
    if ((Test-PathMatch -Path $Path -Rule '.kiro/specs/NPC') -or $comparisonPath -match '(^|/)npc(/|$)') { return 'NPC' }
    if ((Test-PathMatch -Path $Path -Rule '.kiro/specs/999_全面重构_26.03.15/导航检查') -or $comparisonPath -match '(^|/)(navigation|nav)(/|$)') { return '导航检查' }
    if ((Test-PathMatch -Path $Path -Rule '.kiro/specs/999_全面重构_26.03.15/遮挡检查') -or (Test-PathMatch -Path $Path -Rule '.kiro/specs/云朵遮挡系统') -or $comparisonPath -match 'occlusion|遮挡') { return '遮挡检查' }
    if ((Test-PathMatch -Path $Path -Rule '.kiro/specs/900_开篇/spring-day1-implementation') -or $comparisonPath -match 'spring[-_]?day1|springday1') { return 'spring-day1' }
    if ((Test-PathMatch -Path $Path -Rule '.kiro/specs/900_开篇/5.0.0场景搭建') -or $comparisonPath -match 'scenebuild') { return 'scene-build' }
    if (-not [string]::IsNullOrWhiteSpace($OwnerThread)) { return $OwnerThread }
    if (-not [string]::IsNullOrWhiteSpace($Branch) -and $Branch.ToLowerInvariant().StartsWith('codex/')) { return $Branch }

    return 'unknown-owner'
}

function Get-DirtyPolicyHint {
    param([string]$DirtyLevel)

    switch ($DirtyLevel) {
        'D0' { return 'ignored-runtime-no-block' }
        'D1' { return 'governance-white-listable' }
        'D2' { return 'same-thread-branch-only' }
        default { return 'hard-block-cleanup-required' }
    }
}

function New-DirtyReportEntry {
    param(
        [string]$Path,
        [string]$Status,
        [string]$Category,
        [string]$Branch,
        [string]$Mode,
        [string]$OwnerThread
    )

    $dirtyLevel = Get-DirtyLevel -Path $Path -Category $Category -Branch $Branch -Mode $Mode
    $ownerHint = Get-DirtyOwnerHint -Path $Path -Category $Category -Branch $Branch -Mode $Mode -OwnerThread $OwnerThread
    $policyHint = Get-DirtyPolicyHint -DirtyLevel $dirtyLevel

    return [PSCustomObject]@{
        Path       = $Path
        Status     = $Status
        Category   = $Category
        DirtyLevel = $dirtyLevel
        OwnerHint  = $ownerHint
        PolicyHint = $policyHint
    }
}

function Format-DirtyReportEntry {
    param($Entry)

    $details = @()
    if ($null -ne $Entry -and $Entry.PSObject.Properties['Category']) {
        $details += [string]$Entry.Category
    }
    if ($null -ne $Entry -and $Entry.PSObject.Properties['DirtyLevel']) {
        $details += "level=$($Entry.DirtyLevel)"
    }
    if ($null -ne $Entry -and $Entry.PSObject.Properties['OwnerHint']) {
        $details += "owner=$($Entry.OwnerHint)"
    }
    if ($null -ne $Entry -and $Entry.PSObject.Properties['PolicyHint']) {
        $details += "policy=$($Entry.PolicyHint)"
    }

    return "- [$($Entry.Status)] $($Entry.Path) <$($details -join ' | ')>"
}

function Get-DirtyLevelSummaryText {
    param([object[]]$Entries)

    if ($null -eq $Entries -or @($Entries).Count -eq 0) {
        return 'none'
    }

    $parts = @()
    foreach ($level in @('D0', 'D1', 'D2', 'D3')) {
        $count = @($Entries | Where-Object { $null -ne $_ -and $_.DirtyLevel -eq $level }).Count
        if ($count -gt 0) {
            $parts += ('{0}={1}' -f $level, $count)
        }
    }

    if ($parts.Count -eq 0) {
        return 'none'
    }

    return ($parts -join ', ')
}

function Get-RemainingDirtyEntries {
    param([object[]]$Entries)

    if ($null -eq $Entries -or @($Entries).Count -eq 0) {
        return @()
    }

    $filtered = @(
        $Entries | Where-Object {
            $null -ne $_ -and
            -not [string]::IsNullOrWhiteSpace($_.Path) -and
            $_.Category -ne '已知本地噪音' -and
            $_.Category -ne 'shared root 租约运行态脏改'
        }
    )

    return @($filtered)
}

function Test-OnlySharedRootLeaseRuntimeDirtyEntries {
    param(
        [object[]]$Entries,
        [string]$Branch,
        $Occupancy
    )

    if ($null -eq $Entries -or @($Entries).Count -eq 0) {
        return $false
    }

    $runtimeDirty = @(
        $Entries | Where-Object {
            $null -ne $_ -and
            -not [string]::IsNullOrWhiteSpace($_.Path) -and
            (Test-SharedRootLeaseRuntimeDirtyPath -Path $_.Path -Branch $Branch -Occupancy $Occupancy)
        }
    )

    return ($runtimeDirty.Count -gt 0 -and $runtimeDirty.Count -eq @($Entries).Count)
}

function Get-RemainingDirtyGateMessage {
    param([object[]]$Entries)

    $preview = @($Entries | Select-Object -First 5 | ForEach-Object {
            if ($null -ne $_.PSObject.Properties['DirtyLevel']) {
                "$($_.Status) $($_.Path) <$($_.DirtyLevel)/$($_.OwnerHint)>"
            }
            else {
                "$($_.Status) $($_.Path)"
            }
        })
    $suffix = ''
    if (@($Entries).Count -gt 5) {
        $suffix = ' ...'
    }

    return "FATAL: shared root 当前仍存在未纳入本轮白名单的 remaining dirty，task 模式禁止继续写入或同步。请先清尾、归属或隔离这些改动：$($preview -join '; ')$suffix"
}

function Get-SharedRootBranchRequestGate {
    param(
        [string]$OwnerThread,
        [string]$TargetBranch
    )

    $repoRoot = Get-RepoRoot
    $branch = Get-CurrentBranch
    $occupancy = Get-SharedRootOccupancyRecord

    if (-not (Test-IsSharedRootWorkspace -RepoRoot $repoRoot -OccupancyRecord $occupancy)) {
        throw 'FATAL: 只允许在 shared root 上请求分支租约。'
    }

    Assert-TaskBranchMatchesOwner -Branch $TargetBranch -OwnerThread $OwnerThread

    if ($branch -ne 'main') {
        return [PSCustomObject]@{
            CanGrant  = $false
            Code      = 'LOCKED'
            Reason    = "当前 live 分支是 '$branch'，只有 main 大厅才能发放分支租约。"
            Occupancy = $occupancy
        }
    }

    $entries = @(Get-BlockingStatusEntries -Entries (Get-StatusEntries) -Branch $branch -Occupancy $occupancy)
    if ($entries.Count -gt 0) {
        return [PSCustomObject]@{
            CanGrant  = $false
            Code      = 'LOCKED'
            Reason    = 'shared root 当前不干净，暂时不能发放分支租约。'
            Occupancy = $occupancy
        }
    }

    if ($null -eq $occupancy) {
        throw 'FATAL: shared root 缺少 occupancy 文档，禁止 request-branch。'
    }

    if (-not $occupancy.IsNeutral -or $occupancy.CurrentBranch -ne 'main') {
        return [PSCustomObject]@{
            CanGrant  = $false
            Code      = 'LOCKED'
            Reason    = "shared root 当前不是 neutral main 状态（current_branch='$($occupancy.CurrentBranch)'，is_neutral='$($occupancy.IsNeutral)'）。"
            Occupancy = $occupancy
        }
    }

    if ($occupancy.BranchGrantState -eq 'granted' -and $occupancy.BranchGrantOwner -eq $OwnerThread -and $occupancy.BranchGrantBranch -eq $TargetBranch) {
        return [PSCustomObject]@{
            CanGrant  = $false
            Code      = 'ALREADY_GRANTED'
            Reason    = '当前请求对应的分支租约已经发给你，可直接执行 ensure-branch。'
            Occupancy = $occupancy
        }
    }

    if ($occupancy.BranchGrantState -eq 'granted' -and $occupancy.BranchGrantOwner -ne 'none') {
        return [PSCustomObject]@{
            CanGrant  = $false
            Code      = 'LOCKED'
            Reason    = "shared root 已存在未消费的分支租约（owner='$($occupancy.BranchGrantOwner)'，branch='$($occupancy.BranchGrantBranch)'）。"
            Occupancy = $occupancy
        }
    }

    return [PSCustomObject]@{
        CanGrant  = $true
        Code      = 'READY'
        Reason    = 'shared root 当前允许发放分支租约。'
        Occupancy = $occupancy
    }
}

function Request-SharedRootBranchLease {
    param(
        [string]$OwnerThread,
        [string]$TargetBranch,
        [string]$CheckpointHint,
        [string]$Note
    )

    $gate = Get-SharedRootBranchRequestGate -OwnerThread $OwnerThread -TargetBranch $TargetBranch
    $queue = Get-SharedRootQueueRecord

    if ($gate.Code -eq 'ALREADY_GRANTED') {
        $entry = Upsert-SharedRootQueueEntry -Queue $queue -OwnerThread $OwnerThread -TargetBranch $TargetBranch -NewState 'granted' -CheckpointHint $CheckpointHint -Note $Note -LastReason $gate.Reason
        $queue.CurrentServingTicket = [int]$entry.Ticket
        Set-SharedRootQueueRecord -Queue $queue
        $policy = Get-SharedRootHoldWindowPolicy -CheckpointHint $entry.CheckpointHint -Note $entry.Note -TargetBranch $TargetBranch

        Write-Output 'STATUS: ALREADY_GRANTED'
        Write-Output "OWNER_THREAD: $OwnerThread"
        Write-Output "BRANCH: $TargetBranch"
        Write-Output "TICKET: $($entry.Ticket)"
        Write-Output "CHECKPOINT_HINT: $($entry.CheckpointHint)"
        Write-Output "QUEUE_NOTE: $($entry.Note)"
        Write-Output "RECOMMENDED_HOLD_MINUTES: $($policy.MaxMinutes)"
        Write-Output "HOLD_CATEGORY: $($policy.Category)"
        Write-Output "HOLD_SUMMARY: $($policy.Summary)"
        Write-Output "QUEUE_RUNTIME_PATH: $($queue.RuntimePath)"
        Write-Output "QUEUE_SPEC_PATH: $($queue.SpecPath)"
        Write-SharedRootDraftHints -OwnerThread $OwnerThread -TargetBranch $TargetBranch
        Write-Output "MESSAGE: $($gate.Reason)"
        Write-Output 'NEXT_ACTION: 继续 live preflight 后即可 ensure-branch；若等待期已在 Draft 沙盒准备好草稿，请只迁入最小必要内容并快进快出。'
        return
    }

    $entry = Upsert-SharedRootQueueEntry -Queue $queue -OwnerThread $OwnerThread -TargetBranch $TargetBranch -NewState 'waiting' -CheckpointHint $CheckpointHint -Note $Note -LastReason $gate.Reason
    Set-SharedRootQueueRecord -Queue $queue

    $queue = Get-SharedRootQueueRecord
    $entry = Get-OpenSharedRootQueueEntry -Queue $queue -OwnerThread $OwnerThread -TargetBranch $TargetBranch
    $waitingEntries = @(Get-SharedRootWaitingEntries -Queue $queue)
    $waitingAhead = @($waitingEntries | Where-Object { $_.Ticket -lt $entry.Ticket })

    if ($gate.CanGrant -and $waitingAhead.Count -eq 0) {
        Grant-SharedRootBranchLease -OwnerThread $OwnerThread -TargetBranch $TargetBranch
        $queue = Get-SharedRootQueueRecord
        $entry = Get-OpenSharedRootQueueEntry -Queue $queue -OwnerThread $OwnerThread -TargetBranch $TargetBranch
        $policy = Get-SharedRootHoldWindowPolicy -CheckpointHint $entry.CheckpointHint -Note $entry.Note -TargetBranch $TargetBranch

        Write-Output 'STATUS: GRANTED'
        Write-Output "OWNER_THREAD: $OwnerThread"
        Write-Output "BRANCH: $TargetBranch"
        Write-Output "TICKET: $($entry.Ticket)"
        Write-Output "CHECKPOINT_HINT: $($entry.CheckpointHint)"
        Write-Output "QUEUE_NOTE: $($entry.Note)"
        Write-Output "RECOMMENDED_HOLD_MINUTES: $($policy.MaxMinutes)"
        Write-Output "HOLD_CATEGORY: $($policy.Category)"
        Write-Output "HOLD_SUMMARY: $($policy.Summary)"
        Write-Output "QUEUE_RUNTIME_PATH: $($queue.RuntimePath)"
        Write-Output "QUEUE_SPEC_PATH: $($queue.SpecPath)"
        Write-SharedRootDraftHints -OwnerThread $OwnerThread -TargetBranch $TargetBranch
        Write-Output "MESSAGE: 已发放 shared root 分支租约，可继续执行 ensure-branch。"
        Write-Output 'NEXT_ACTION: 继续 ensure-branch；如果等待期已在 Draft 沙盒准备好草稿，只迁入本轮最小 checkpoint 所需内容。'
        return
    }

    $reason = $gate.Reason
    if ($gate.CanGrant -and $waitingAhead.Count -gt 0) {
        $head = $waitingAhead | Sort-Object Ticket | Select-Object -First 1
        $reason = "前方仍有排队线程：ticket='$($head.Ticket)' owner_thread='$($head.OwnerThread)' target_branch='$($head.TargetBranch)'。"
        $entry = Upsert-SharedRootQueueEntry -Queue $queue -OwnerThread $OwnerThread -TargetBranch $TargetBranch -NewState 'waiting' -CheckpointHint $CheckpointHint -Note $Note -LastReason $reason
        Set-SharedRootQueueRecord -Queue $queue
    }

    $queuePosition = (@($waitingEntries | Where-Object { $_.Ticket -le $entry.Ticket })).Count

    Write-Output 'STATUS: LOCKED_PLEASE_YIELD'
    Write-Output "OWNER_THREAD: $OwnerThread"
    Write-Output "BRANCH: $TargetBranch"
    Write-Output "TICKET: $($entry.Ticket)"
    Write-Output "QUEUE_POSITION: $queuePosition"
    Write-Output "CHECKPOINT_HINT: $($entry.CheckpointHint)"
    Write-Output "QUEUE_NOTE: $($entry.Note)"
    Write-Output "QUEUE_RUNTIME_PATH: $($queue.RuntimePath)"
    Write-Output "QUEUE_SPEC_PATH: $($queue.SpecPath)"
    Write-SharedRootDraftHints -OwnerThread $OwnerThread -TargetBranch $TargetBranch
    Write-Output "REASON: $reason"
    Write-Output 'NEXT_ACTION: 停止继续尝试 ensure-branch；不要在 main 上写 tracked memory / 回执；如需保存恢复点或代码草稿，请使用 CheckpointHint / QueueNote / Draft 沙盒 / 最小聊天回执，进入等待态。'
}

function Grant-SharedRootBranchLease {
    param(
        [string]$OwnerThread,
        [string]$TargetBranch
    )

    $repoRoot = Get-RepoRoot
    $branch = Get-CurrentBranch
    $occupancy = Get-SharedRootOccupancyRecord

    if (-not (Test-IsSharedRootWorkspace -RepoRoot $repoRoot -OccupancyRecord $occupancy)) {
        throw 'FATAL: 只允许在 shared root 上发放分支租约。'
    }

    if ($branch -ne 'main') {
        throw "FATAL: 当前 live 分支是 '$branch'，只有 main 大厅才能发放分支租约。"
    }

    Assert-TaskBranchMatchesOwner -Branch $TargetBranch -OwnerThread $OwnerThread

    $entries = @(Get-BlockingStatusEntries -Entries (Get-StatusEntries) -Branch $branch -Occupancy $occupancy)
    if ($entries.Count -gt 0) {
        throw 'FATAL: shared root 当前不干净，禁止发放分支租约。'
    }

    if ($null -eq $occupancy) {
        throw 'FATAL: shared root 缺少 occupancy 文档，禁止发放分支租约。'
    }

    if (-not $occupancy.IsNeutral -or $occupancy.CurrentBranch -ne 'main') {
        throw "FATAL: shared root 当前不是 neutral main 状态（current_branch='$($occupancy.CurrentBranch)'，is_neutral='$($occupancy.IsNeutral)'），禁止发放分支租约。"
    }

    if ($occupancy.BranchGrantState -eq 'granted' -and $occupancy.BranchGrantOwner -ne 'none') {
        throw "FATAL: shared root 已存在未消费的分支租约（owner='$($occupancy.BranchGrantOwner)'，branch='$($occupancy.BranchGrantBranch)'），禁止重复发放。"
    }

    $queue = Get-SharedRootQueueRecord
    $headWaiting = @(Get-SharedRootWaitingEntries -Queue $queue | Select-Object -First 1)
    if ($headWaiting.Count -gt 0) {
        $head = $headWaiting[0]
        if ($head.OwnerThread -ne $OwnerThread -or $head.TargetBranch -ne $TargetBranch) {
            throw "FATAL: shared root 当前队首属于 ticket='$($head.Ticket)' owner_thread='$($head.OwnerThread)' target_branch='$($head.TargetBranch)'，当前请求不得插队。"
        }
    }

    Set-SharedRootOccupancyValues -Values ([ordered]@{
            owner_mode                = 'neutral-main-ready'
            owner_thread              = 'none'
            current_branch            = 'main'
            last_verified_head        = (Get-HeadShort)
            is_neutral                = 'true'
            lease_state               = 'branch-granted'
            branch_grant_state        = 'granted'
            branch_grant_owner_thread = $OwnerThread
            branch_grant_branch       = $TargetBranch
            branch_grant_updated      = (Get-OccupancyTimestamp)
            blocking_dirty_scope      = 'none'
            last_updated              = (Get-OccupancyDateStamp)
        })

    $entry = Get-OpenSharedRootQueueEntry -Queue $queue -OwnerThread $OwnerThread -TargetBranch $TargetBranch
    if ($null -ne $entry) {
        $entry = Upsert-SharedRootQueueEntry -Queue $queue -OwnerThread $OwnerThread -TargetBranch $TargetBranch -NewState 'granted' -CheckpointHint $entry.CheckpointHint -Note $entry.Note -LastReason 'grant-issued'
        $queue.CurrentServingTicket = [int]$entry.Ticket
        Set-SharedRootQueueRecord -Queue $queue
    }

    Write-Output "已向 OwnerThread '$OwnerThread' 发放 shared root 分支租约：$TargetBranch"
}

function Return-SharedRootToMain {
    param([string]$OwnerThread)

    $repoRoot = Get-RepoRoot
    $occupancy = Get-SharedRootOccupancyRecord

    if (-not (Test-IsSharedRootWorkspace -RepoRoot $repoRoot -OccupancyRecord $occupancy)) {
        throw 'FATAL: 只允许在 shared root 上执行 return-main。'
    }

    $branch = Get-CurrentBranch
    if ($branch -eq 'main' -and $null -ne $occupancy -and $occupancy.BranchGrantState -eq 'granted' -and $occupancy.BranchGrantOwner -ne 'none' -and $occupancy.BranchGrantOwner -ne $OwnerThread) {
        throw "FATAL: shared root 当前未消费租约属于 '$($occupancy.BranchGrantOwner)'，'$OwnerThread' 不得越权清空。"
    }

    $allStatusEntries = @(Get-StatusEntries)
    $entries = @(Get-BlockingStatusEntries -Entries $allStatusEntries -Branch $branch -Occupancy $occupancy)
    if ($entries.Count -gt 0) {
        throw 'FATAL: 当前工作树不干净，禁止 return-main。请先提交、推送或清理当前任务脏改。'
    }

    $taskBranch = $branch

    if ($branch -ne 'main') {
        Assert-TaskBranchMatchesOwner -Branch $branch -OwnerThread $OwnerThread

        if ($null -ne $occupancy) {
            if ($occupancy.OwnerThread -ne $OwnerThread -or $occupancy.CurrentBranch -ne $branch) {
                throw "FATAL: occupancy 文档登记的是 owner_thread='$($occupancy.OwnerThread)' / current_branch='$($occupancy.CurrentBranch)'，与当前 '$OwnerThread' / '$branch' 不符，禁止 return-main。"
            }
        }

        $checkoutArgs = @('checkout', 'main')
        if (Test-OnlySharedRootLeaseRuntimeDirtyEntries -Entries $allStatusEntries -Branch $branch -Occupancy $occupancy) {
            $checkoutArgs = @('checkout', '-f', 'main')
        }

        Invoke-Git -Arguments $checkoutArgs | Out-Null
    }

    $sessionSummary = $null
    if ($taskBranch -ne 'main') {
        $sessionSummary = Complete-SharedRootActiveSession -OwnerThread $OwnerThread -TargetBranch $taskBranch -CompletionStatus 'returned-main'
    }

    Set-SharedRootNeutralState
    if ($taskBranch -ne 'main') {
        Complete-SharedRootQueueEntry -OwnerThread $OwnerThread -TargetBranch $taskBranch
    }
    $queue = Get-SharedRootQueueRecord
    $nextWaiting = Get-SharedRootNextWaitingEntry -Queue $queue

    Write-Output 'STATUS: RETURNED_MAIN'
    Write-Output 'MESSAGE: 已归还 shared root 到 main，并清空分支租约。'
    Write-SharedRootDraftHints -OwnerThread $OwnerThread -TargetBranch $taskBranch
    if ($null -ne $sessionSummary) {
        Write-Output "ACTIVE_HOLD_CATEGORY: $($sessionSummary.hold_category)"
        Write-Output "ACTIVE_RECOMMENDED_HOLD_MINUTES: $($sessionSummary.recommended_hold_minutes)"
        Write-Output "ACTIVE_ACTUAL_HOLD_MINUTES: $($sessionSummary.actual_hold_minutes)"
        Write-Output "ACTIVE_HOLD_BUDGET_STATUS: $($sessionSummary.budget_status)"
    }
    if ($null -ne $nextWaiting) {
        Write-Output "NEXT_IN_LINE_TICKET: $($nextWaiting.Ticket)"
        Write-Output "NEXT_IN_LINE_OWNER_THREAD: $($nextWaiting.OwnerThread)"
        Write-Output "NEXT_IN_LINE_BRANCH: $($nextWaiting.TargetBranch)"
        Write-Output "NEXT_IN_LINE_CHECKPOINT_HINT: $($nextWaiting.CheckpointHint)"
        Write-Output "NEXT_IN_LINE_NOTE: $($nextWaiting.Note)"
        Write-Output 'POST_RETURN_EVIDENCE_MODE: defer-tracked-while-queue-waiting'
        Write-Output 'POST_RETURN_NEXT_ACTION: shared root 已释放，但队列仍有人等待；不要立刻在 main 上写 tracked memory / 回执。请先把复盘或草稿放进 Draft 沙盒或最小聊天回执，待治理窗口再做最小白名单 sync。'
        Write-Output 'NEXT_ACTION: 治理线程可执行 wake-next，或向队首线程发放唤醒 Prompt。'
    }
    else {
        Write-Output 'NEXT_IN_LINE_TICKET: none'
        Write-Output 'POST_RETURN_EVIDENCE_MODE: minimal-tracked-allowed'
        Write-Output 'POST_RETURN_NEXT_ACTION: shared root 已释放且当前无 waiting 条目；如需补写 tracked memory / 回执，请保持最小修改并尽快用白名单 sync 收口。'
        Write-Output 'NEXT_ACTION: 当前队列无 waiting 条目，shared root 保持 neutral 待机。'
    }
}

function Test-TaskSharedRootLease {
    param(
        [string]$RepoRoot,
        [string]$Branch,
        [string]$OwnerThread,
        [object[]]$RemainingEntries
    )

    $occupancy = Get-SharedRootOccupancyRecord

    if (-not (Test-IsSharedRootWorkspace -RepoRoot $RepoRoot -OccupancyRecord $occupancy)) {
        return [PSCustomObject]@{
            Applies   = $false
            CanUse    = $true
            Reason    = '当前不在 shared root，不适用 shared root lease 闸机。'
            Occupancy = $occupancy
        }
    }

    $remainingDirty = @(Get-RemainingDirtyEntries -Entries $RemainingEntries)
    $isMainOnlyTaskSync = $Branch -eq 'main'

    if ($null -eq $occupancy) {
        return [PSCustomObject]@{
            Applies   = $true
            CanUse    = ($isMainOnlyTaskSync -or $remainingDirty.Count -eq 0)
            Reason    = if ($isMainOnlyTaskSync) {
                'shared root 未发现 occupancy 文档；当前按 main-only 白名单 sync 继续。'
            }
            elseif ($remainingDirty.Count -eq 0) {
                'shared root 未发现 occupancy 文档，lease 校验暂时跳过。'
            }
            else {
                Get-RemainingDirtyGateMessage -Entries $remainingDirty
            }
            Occupancy = $null
        }
    }

    if (-not $occupancy.IsNeutral) {
        if ([string]::IsNullOrWhiteSpace($occupancy.CurrentBranch)) {
            return [PSCustomObject]@{
                Applies   = $true
                CanUse    = $false
                Reason    = "FATAL: shared root 占用文档 '$($occupancy.OccupancyPath)' 已声明非中性，但未写明 current_branch。"
                Occupancy = $occupancy
            }
        }

        if ($occupancy.CurrentBranch -ne $Branch) {
            return [PSCustomObject]@{
                Applies   = $true
                CanUse    = $false
                Reason    = "FATAL: shared root 占用文档声明 current_branch='$($occupancy.CurrentBranch)'，但 live 分支是 '$Branch'；请先回正或纠偏占用文档。"
                Occupancy = $occupancy
            }
        }

        if ([string]::IsNullOrWhiteSpace($occupancy.OwnerThread)) {
            return [PSCustomObject]@{
                Applies   = $true
                CanUse    = $false
                Reason    = "FATAL: shared root 已声明为 occupied，但占用文档缺少 owner_thread；请先补齐占用人。"
                Occupancy = $occupancy
            }
        }

        if ($occupancy.OwnerThread -ne $OwnerThread) {
            return [PSCustomObject]@{
                Applies   = $true
                CanUse    = $false
                Reason    = "FATAL: shared root 当前登记 owner_thread='$($occupancy.OwnerThread)'，OwnerThread '$OwnerThread' 不得继续在 shared root 上以 task 模式写入。"
                Occupancy = $occupancy
            }
        }
    }

    if ($isMainOnlyTaskSync) {
        return [PSCustomObject]@{
            Applies   = $true
            CanUse    = $true
            Reason    = if ($remainingDirty.Count -gt 0) {
                'shared root 当前位于 main-only 白名单 sync；允许保留 unrelated dirty，不再因 remaining dirty 一刀切阻断。'
            }
            elseif ($occupancy.IsNeutral) {
                'shared root 当前登记为 neutral main，允许 main-only 白名单 sync。'
            }
            else {
                'shared root main-only 白名单 sync 校验通过。'
            }
            Occupancy = $occupancy
        }
    }

    if ($remainingDirty.Count -gt 0) {
        return [PSCustomObject]@{
            Applies   = $true
            CanUse    = $false
            Reason    = Get-RemainingDirtyGateMessage -Entries $remainingDirty
            Occupancy = $occupancy
        }
    }

    return [PSCustomObject]@{
        Applies   = $true
        CanUse    = $true
        Reason    = if ($occupancy.IsNeutral) { 'shared root 当前登记为 neutral，且没有剩余 dirty 阻塞。' } else { 'shared root owner/lease 校验通过，且没有剩余 dirty 阻塞。' }
        Occupancy = $occupancy
    }
}

function New-PreflightReport {
    param(
        [string]$Mode,
        [string]$OwnerThread,
        [string[]]$ScopeRoots,
        [string[]]$IncludePaths
    )

    $repoRoot = Get-RepoRoot
    $branch = Get-CurrentBranch
    $head = Get-HeadShort
    $upstream = Get-UpstreamCounts
    $occupancy = Get-SharedRootOccupancyRecord
    $entries = Get-StatusEntries
    $allowed = @()
    $remaining = @()

    foreach ($entry in $entries) {
        if (Test-NoisePath -Path $entry.Path) {
            $remaining += New-DirtyReportEntry -Path $entry.Path -Status $entry.Status -Category '已知本地噪音' -Branch $branch -Mode $Mode -OwnerThread $OwnerThread
            continue
        }

        if (Test-SharedRootLeaseRuntimeDirtyPath -Path $entry.Path -Branch $branch -Occupancy $occupancy) {
            $remaining += New-DirtyReportEntry -Path $entry.Path -Status $entry.Status -Category 'shared root 租约运行态脏改' -Branch $branch -Mode $Mode -OwnerThread $OwnerThread
            continue
        }

        if (Test-AllowedPath -Path $entry.Path -Mode $Mode -ScopeRoots $ScopeRoots -IncludePaths $IncludePaths) {
            $allowed += New-DirtyReportEntry -Path $entry.Path -Status $entry.Status -Category (Get-PathGroup -Path $entry.Path) -Branch $branch -Mode $Mode -OwnerThread $OwnerThread
        }
        else {
            $remaining += New-DirtyReportEntry -Path $entry.Path -Status $entry.Status -Category (Get-PathGroup -Path $entry.Path) -Branch $branch -Mode $Mode -OwnerThread $OwnerThread
        }
    }

    $canContinue = $true
    $reason = '允许继续'
    $codeGuard = New-EmptyCodeGuardReport -Reason '当前模式未运行代码闸门。'

    $sharedRootLease = $null
    if ($Mode -eq 'task') {
        $sharedRootLease = Test-TaskSharedRootLease -RepoRoot $repoRoot -Branch $branch -OwnerThread $OwnerThread -RemainingEntries $remaining
    }
    else {
        $sharedRootLease = [PSCustomObject]@{
            Applies   = $false
            CanUse    = $true
            Reason    = 'governance 模式不消费 shared root task lease；只校验白名单同步边界。'
            Occupancy = $occupancy
        }
    }

    $isMainOnlyTaskSync = $Mode -eq 'task' -and $branch -eq 'main'

    if ($Mode -eq 'task' -and -not $isMainOnlyTaskSync -and -not $branch.ToLowerInvariant().StartsWith('codex/')) {
        $canContinue = $false
        $reason = "FATAL: 当前分支 '$branch' 不是 codex/ 前缀，真实任务禁止继续写入。"
    }
    elseif ($Mode -eq 'task' -and -not $isMainOnlyTaskSync -and -not (Test-BranchOwnedByThread -Branch $branch -OwnerThread $OwnerThread)) {
        $canContinue = $false
        $reason = Get-BranchOwnershipMessage -Branch $branch -OwnerThread $OwnerThread
    }
    elseif ($Mode -eq 'task' -and ((@(Expand-PathList -Paths $ScopeRoots).Count -eq 0) -and (@(Expand-PathList -Paths $IncludePaths).Count -eq 0))) {
        $canContinue = $false
        $reason = "FATAL: OwnerThread '$OwnerThread' 在 task 模式下必须提供 ScopeRoots 或 IncludePaths，禁止无边界自动提交。"
    }
    elseif ($Mode -eq 'task' -and -not $sharedRootLease.CanUse) {
        $canContinue = $false
        $reason = $sharedRootLease.Reason
    }

    if ($canContinue -and $Mode -eq 'task') {
        $codeGuard = Invoke-CodeGuard -RepoRoot $repoRoot -OwnerThread $OwnerThread -Branch $branch -AllowedEntries $allowed -Phase 'pre-sync'
        if ($codeGuard.Applies -and -not $codeGuard.CanContinue) {
            $canContinue = $false
            $reason = "FATAL: 代码闸门未通过：$($codeGuard.Reason)"
        }
    }

    return [PSCustomObject]@{
        OwnerThread     = $OwnerThread
        RepoRoot        = $repoRoot
        Branch          = $branch
        Head            = $head
        Upstream        = $upstream
        Allowed         = @($allowed)
        Remaining       = @($remaining)
        SharedRootLease = $sharedRootLease
        CodeGuard       = $codeGuard
        CanContinue     = $canContinue
        Reason          = $reason
    }
}

function Write-PreflightReport {
    param($Report)

    Write-Output "OwnerThread: $($Report.OwnerThread)"
    Write-Output "仓库根目录: $($Report.RepoRoot)"
    Write-Output "当前分支: $($Report.Branch)"
    Write-Output "当前 HEAD: $($Report.Head)"
    Write-Output "upstream 状态 ($($Report.Upstream.Label)): behind=$($Report.Upstream.Behind), ahead=$($Report.Upstream.Ahead)"
    Write-Output "是否允许按当前模式继续: $($Report.CanContinue)"
    Write-Output "判断原因: $($Report.Reason)"

    if ($null -ne $Report.CodeGuard) {
        Write-Output "代码闸门适用: $($Report.CodeGuard.Applies)"
        Write-Output "代码闸门通过: $($Report.CodeGuard.CanContinue)"
        Write-Output "代码闸门摘要: $($Report.CodeGuard.Summary)"
        Write-Output "代码闸门原因: $($Report.CodeGuard.Reason)"
        if (@($Report.CodeGuard.ChangedCodeFiles).Count -gt 0) {
            Write-Output "代码闸门文件数: $(@($Report.CodeGuard.ChangedCodeFiles).Count)"
        }
        if (@($Report.CodeGuard.AffectedAssemblies).Count -gt 0) {
            Write-Output "代码闸门程序集: $((@($Report.CodeGuard.AffectedAssemblies) -join ', '))"
        }
        if (@($Report.CodeGuard.Diagnostics).Count -gt 0) {
            Write-Output '代码闸门诊断:'
            foreach ($diagnostic in @($Report.CodeGuard.Diagnostics)) {
                $location = 'n/a'
                if (-not [string]::IsNullOrWhiteSpace($diagnostic.FilePath)) {
                    $location = $diagnostic.FilePath
                    if ($null -ne $diagnostic.Line) {
                        $location += ":$($diagnostic.Line)"
                        if ($null -ne $diagnostic.Column) {
                            $location += ":$($diagnostic.Column)"
                        }
                    }
                }
                $assemblySegment = if (-not [string]::IsNullOrWhiteSpace($diagnostic.Assembly)) { " [$($diagnostic.Assembly)]" } else { '' }
                Write-Output "- [$($diagnostic.Severity)] $($diagnostic.RuleId)$assemblySegment $location :: $($diagnostic.Message)"
            }
        }
    }

    if ($null -ne $Report.SharedRootLease -and $Report.SharedRootLease.Applies) {
        Write-Output "shared root lease 判断: $($Report.SharedRootLease.CanUse)"
        Write-Output "shared root lease 原因: $($Report.SharedRootLease.Reason)"

        if ($null -ne $Report.SharedRootLease.Occupancy) {
            Write-Output "shared root owner_mode: $($Report.SharedRootLease.Occupancy.OwnerMode)"
            Write-Output "shared root owner_thread: $($Report.SharedRootLease.Occupancy.OwnerThread)"
            Write-Output "shared root current_branch: $($Report.SharedRootLease.Occupancy.CurrentBranch)"
            Write-Output "shared root is_neutral: $($Report.SharedRootLease.Occupancy.IsNeutral)"
            Write-Output "shared root lease_state: $($Report.SharedRootLease.Occupancy.LeaseState)"
            Write-Output "shared root branch_grant_state: $($Report.SharedRootLease.Occupancy.BranchGrantState)"
            Write-Output "shared root branch_grant_owner: $($Report.SharedRootLease.Occupancy.BranchGrantOwner)"
            Write-Output "shared root branch_grant_branch: $($Report.SharedRootLease.Occupancy.BranchGrantBranch)"
        }
    }

    Write-Output ''
    Write-Output '本轮允许纳入同步的改动:'
    Write-Output "允许项 dirty 分级概览: $(Get-DirtyLevelSummaryText -Entries $Report.Allowed)"

    if (@($Report.Allowed).Count -eq 0) {
        Write-Output '- （无）'
    }
    else {
        foreach ($item in $Report.Allowed) {
            Write-Output (Format-DirtyReportEntry -Entry $item)
        }
    }

    Write-Output ''
    Write-Output '本轮不会自动纳入同步、只做保留/报告的改动:'
    Write-Output "剩余项 dirty 分级概览: $(Get-DirtyLevelSummaryText -Entries $Report.Remaining)"

    if (@($Report.Remaining).Count -eq 0) {
        Write-Output '- （无）'
    }
    else {
        foreach ($item in $Report.Remaining) {
            Write-Output (Format-DirtyReportEntry -Entry $item)
        }
    }
}

function Get-NextCommitMessage {
    param([string]$OwnerThread)

    $prefix = Get-Date -Format 'yyyy.MM.dd'
    $threadSegment = if ([string]::IsNullOrWhiteSpace($OwnerThread)) {
        'unknown-thread'
    }
    else {
        (($OwnerThread.Trim()) -replace '\s+', '-') -replace '[\\/:"*?<>|]+', '-'
    }
    $messagePrefix = '{0}_{1}_' -f $prefix, $threadSegment
    $since = (Get-Date).ToString('yyyy-MM-dd') + ' 00:00'
    $messages = (Invoke-Git -Arguments @('log', "--since=$since", '--format=%s')).Output
    $numbers = @()

    foreach ($message in $messages) {
        if ($message -match "^$([regex]::Escape($messagePrefix))(\d{2})$") {
            $numbers += [int]$Matches[1]
        }
    }

    $next = 1
    if ($numbers.Count -gt 0) {
        $next = (($numbers | Measure-Object -Maximum).Maximum) + 1
    }

    return ('{0}{1}' -f $messagePrefix, ('{0:D2}' -f [int]$next))
}

function Stage-Paths {
    param([string[]]$Paths)

    $uniquePaths = @($Paths | Sort-Object -Unique | Where-Object { -not [string]::IsNullOrWhiteSpace($_) })
    if ($uniquePaths.Count -eq 0) {
        return
    }

    $existingPaths = @()
    $missingPaths = @()

    foreach ($path in $uniquePaths) {
        if (Test-Path -LiteralPath $path) {
            $existingPaths += $path
        }
        else {
            $cachedStatus = Invoke-Git -Arguments @('diff', '--cached', '--name-only', '--', $path)
            $isAlreadyStaged = @($cachedStatus.Output | Where-Object { (Normalize-InputPath -Path $_) -eq (Normalize-InputPath -Path $path) }).Count -gt 0

            if ($isAlreadyStaged) {
                continue
            }

            $trackedStatus = Invoke-Git -Arguments @('ls-files', '--error-unmatch', '--', $path) -AllowFailure
            if ($trackedStatus.ExitCode -eq 0) {
                $missingPaths += $path
            }
        }
    }

    if ($existingPaths.Count -gt 0) {
        Invoke-Git -Arguments (@('add', '--') + $existingPaths) | Out-Null
    }

    if ($missingPaths.Count -gt 0) {
        Invoke-Git -Arguments (@('add', '-u', '--') + $missingPaths) | Out-Null
    }
}

function Ensure-TaskBranch {
    param(
        [string]$TargetBranch,
        [string]$OwnerThread
    )

    if (-not $TargetBranch.ToLowerInvariant().StartsWith('codex/')) {
        throw "FATAL: 目标分支 '$TargetBranch' 不是 codex/ 前缀，禁止创建或切换。"
    }

    Assert-TaskBranchMatchesOwner -Branch $TargetBranch -OwnerThread $OwnerThread

    $branch = Get-CurrentBranch
    $repoRoot = Get-RepoRoot
    $occupancy = Get-SharedRootOccupancyRecord

    if ((Test-IsSharedRootWorkspace -RepoRoot $repoRoot -OccupancyRecord $occupancy) -and $branch -eq 'main') {
        Assert-SharedRootBranchGrant -Occupancy $occupancy -OwnerThread $OwnerThread -TargetBranch $TargetBranch
    }

    if ($branch -eq $TargetBranch) {
        Write-Output "已位于目标分支：$TargetBranch"
        return
    }

    $allStatusEntries = @(Get-StatusEntries)
    $entries = @(Get-BlockingStatusEntries -Entries $allStatusEntries -Branch $branch -Occupancy $occupancy)
    if ($entries.Count -gt 0) {
        throw "当前工作树不干净，不能安全创建/切换到 $TargetBranch。请先收口或隔离现有脏改。"
    }

    $upstream = Get-UpstreamCounts
    if ($branch -eq 'main' -and (($upstream.Ahead -ne 0) -or ($upstream.Behind -ne 0))) {
        throw "当前 main 与远端不同步（behind=$($upstream.Behind), ahead=$($upstream.Ahead)），请先同步后再创建任务分支。"
    }

    $existing = Invoke-Git -Arguments @('branch', '--list', $TargetBranch)
    $checkoutArgs = @()
    if ($existing.Output.Count -gt 0 -and -not [string]::IsNullOrWhiteSpace(($existing.Output -join '').Trim())) {
        $checkoutArgs = @('checkout', $TargetBranch)
        if (Test-OnlySharedRootLeaseRuntimeDirtyEntries -Entries $allStatusEntries -Branch $branch -Occupancy $occupancy) {
            $checkoutArgs = @('checkout', '-f', $TargetBranch)
        }

        Invoke-Git -Arguments $checkoutArgs | Out-Null
        Write-Output "已切换到现有分支：$TargetBranch"
    }
    else {
        $checkoutArgs = @('checkout', '-b', $TargetBranch)
        if (Test-OnlySharedRootLeaseRuntimeDirtyEntries -Entries $allStatusEntries -Branch $branch -Occupancy $occupancy) {
            $checkoutArgs = @('checkout', '-f', '-b', $TargetBranch)
        }

        Invoke-Git -Arguments $checkoutArgs | Out-Null
        Write-Output "已创建并切换到新分支：$TargetBranch"
    }

    if ((Test-IsSharedRootWorkspace -RepoRoot $repoRoot -OccupancyRecord $occupancy) -and $branch -eq 'main') {
        Set-SharedRootTaskActive -OwnerThread $OwnerThread -TargetBranch $TargetBranch
        Write-Output "已消费 shared root 分支租约，并登记为 task-active：$OwnerThread -> $TargetBranch"
        $activeSession = Get-SharedRootActiveSession
        if ($null -ne $activeSession) {
            Write-Output "RUNTIME_ACTIVE_SESSION_PATH: $(Get-SharedRootActiveSessionRuntimePath)"
            Write-Output "RECOMMENDED_HOLD_MINUTES: $($activeSession.recommended_hold_minutes)"
            Write-Output "HOLD_CATEGORY: $($activeSession.hold_category)"
            Write-Output "HOLD_SUMMARY: $($activeSession.hold_summary)"
            Write-Output 'NEXT_ACTION: 在 shared root 中只做最小写事务；长时间只读分析、治理回执和 memory 补记请放到 return-main 之后。'
        }
    }
}

$repoRoot = Get-RepoRoot
Set-Location $repoRoot

switch ($Action) {
    'preflight' {
        $report = New-PreflightReport -Mode $Mode -OwnerThread $OwnerThread -ScopeRoots $ScopeRoots -IncludePaths $IncludePaths
        Write-PreflightReport -Report $report
    }

    'ensure-branch' {
        if ([string]::IsNullOrWhiteSpace($BranchName)) {
            throw 'ensure-branch 必须提供 -BranchName。'
        }

        Ensure-TaskBranch -TargetBranch $BranchName -OwnerThread $OwnerThread
    }

    'grant-branch' {
        if ([string]::IsNullOrWhiteSpace($BranchName)) {
            throw 'grant-branch 必须提供 -BranchName。'
        }

        Grant-SharedRootBranchLease -OwnerThread $OwnerThread -TargetBranch $BranchName
    }

    'request-branch' {
        if ([string]::IsNullOrWhiteSpace($BranchName)) {
            throw 'request-branch 必须提供 -BranchName。'
        }

        Request-SharedRootBranchLease -OwnerThread $OwnerThread -TargetBranch $BranchName -CheckpointHint $CheckpointHint -Note $QueueNote
    }

    'cancel-branch-request' {
        if ([string]::IsNullOrWhiteSpace($BranchName)) {
            throw 'cancel-branch-request 必须提供 -BranchName。'
        }

        Cancel-SharedRootBranchRequest -OwnerThread $OwnerThread -TargetBranch $BranchName -Note $QueueNote
    }

    'requeue-branch-request' {
        if ([string]::IsNullOrWhiteSpace($BranchName)) {
            throw 'requeue-branch-request 必须提供 -BranchName。'
        }

        Requeue-SharedRootBranchRequest -OwnerThread $OwnerThread -TargetBranch $BranchName -CheckpointHint $CheckpointHint -Note $QueueNote
    }

    'wake-next' {
        Wake-NextSharedRootBranchRequest -DispatcherOwnerThread $OwnerThread
    }

    'return-main' {
        Return-SharedRootToMain -OwnerThread $OwnerThread
    }

    'sync' {
        $report = New-PreflightReport -Mode $Mode -OwnerThread $OwnerThread -ScopeRoots $ScopeRoots -IncludePaths $IncludePaths
        Write-PreflightReport -Report $report
        Write-Output ''

        if (-not $report.CanContinue) {
            throw "当前模式不允许继续同步：$($report.Reason)"
        }

        $allowedPaths = @($report.Allowed | ForEach-Object { $_.Path } | Sort-Object -Unique)

        if ($allowedPaths.Count -gt 0) {
            Stage-Paths -Paths $allowedPaths
            Write-Output '已显式暂存允许纳入同步的文件。'
            Write-Output ''
            Write-Output '暂存区概览:'
            (Invoke-Git -Arguments @('diff', '--cached', '--stat')).Output | ForEach-Object { Write-Output $_ }
            Write-Output ''

            $message = Get-NextCommitMessage -OwnerThread $OwnerThread
            Invoke-Git -Arguments @('commit', '-m', $message) | Out-Null
            $newHead = Get-HeadShort
            Write-Output "已创建提交：$message ($newHead)"
        }
        else {
            Write-Output '当前没有新的允许同步改动需要提交。'
        }

        $branch = Get-CurrentBranch
        $upstreamName = Invoke-Git -Arguments @('rev-parse', '--abbrev-ref', '--symbolic-full-name', '@{upstream}') -AllowFailure
        if ($upstreamName.ExitCode -eq 0 -and $upstreamName.Output.Count -gt 0 -and -not [string]::IsNullOrWhiteSpace(($upstreamName.Output[0]).Trim())) {
            Invoke-Git -Arguments @('push') | Out-Null
            Write-Output "已推送当前分支：$branch"
        }
        else {
            Invoke-Git -Arguments @('push', '-u', 'origin', $branch) | Out-Null
            Write-Output "已推送并建立 upstream：$branch"
        }

        $finalHead = Get-HeadShort
        $finalUpstream = Get-UpstreamCounts
        Write-Output "最终 HEAD: $finalHead"
        Write-Output "推送后 upstream 状态 ($($finalUpstream.Label)): behind=$($finalUpstream.Behind), ahead=$($finalUpstream.Ahead)"

        if ($Mode -eq 'task') {
            $currentBranch = Get-CurrentBranch
            $activeSession = Touch-SharedRootActiveSession -OwnerThread $OwnerThread -TargetBranch $currentBranch -Reason 'task-sync'
            if ($null -ne $activeSession) {
                Write-Output "ACTIVE_SESSION_LAST_UPDATED: $($activeSession.last_updated)"
                Write-Output "ACTIVE_SESSION_HOLD_CATEGORY: $($activeSession.hold_category)"
            }
        }

        if ($report.Remaining.Count -gt 0) {
            Write-Output ''
            Write-Output '仍保留在工作树中的其他改动:'
            foreach ($item in $report.Remaining) {
                Write-Output "- [$($item.Status)] $($item.Path) <$($item.Category)>"
            }
        }
    }
}
