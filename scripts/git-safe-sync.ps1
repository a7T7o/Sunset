param(
    [Parameter(Mandatory = $true)]
    [ValidateSet('preflight', 'ensure-branch', 'grant-branch', 'return-main', 'sync')]
    [string]$Action,

    [Parameter(Mandatory = $true)]
    [string]$OwnerThread,

    [ValidateSet('governance', 'task')]
    [string]$Mode = 'task',

    [string]$BranchName,

    [string[]]$ScopeRoots = @(),

    [string[]]$IncludePaths = @()
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
}

function Set-SharedRootNeutralState {
    Set-SharedRootOccupancyValues -Values ([ordered]@{
            owner_mode                = 'neutral-main-ready'
            owner_thread              = 'none'
            current_branch            = 'main'
            last_verified_head        = (Get-HeadShort)
            is_neutral                = 'true'
            lease_state               = 'neutral'
            branch_grant_state        = 'none'
            branch_grant_owner_thread = 'none'
            branch_grant_branch       = 'none'
            branch_grant_updated      = (Get-OccupancyTimestamp)
            blocking_dirty_scope      = 'none'
            last_updated              = (Get-OccupancyDateStamp)
        })
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

function Get-RemainingDirtyGateMessage {
    param([object[]]$Entries)

    $preview = @($Entries | Select-Object -First 5 | ForEach-Object { "$($_.Status) $($_.Path)" })
    $suffix = ''
    if (@($Entries).Count -gt 5) {
        $suffix = ' ...'
    }

    return "FATAL: shared root 当前仍存在未纳入本轮白名单的 remaining dirty，task 模式禁止继续写入或同步。请先清尾、归属或隔离这些改动：$($preview -join '; ')$suffix"
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

    $entries = @(Get-BlockingStatusEntries -Entries (Get-StatusEntries) -Branch $branch -Occupancy $occupancy)
    if ($entries.Count -gt 0) {
        throw 'FATAL: 当前工作树不干净，禁止 return-main。请先提交、推送或清理当前任务脏改。'
    }

    if ($branch -ne 'main') {
        Assert-TaskBranchMatchesOwner -Branch $branch -OwnerThread $OwnerThread

        if ($null -ne $occupancy) {
            if ($occupancy.OwnerThread -ne $OwnerThread -or $occupancy.CurrentBranch -ne $branch) {
                throw "FATAL: occupancy 文档登记的是 owner_thread='$($occupancy.OwnerThread)' / current_branch='$($occupancy.CurrentBranch)'，与当前 '$OwnerThread' / '$branch' 不符，禁止 return-main。"
            }
        }

        Invoke-Git -Arguments @('checkout', 'main') | Out-Null
    }

    Set-SharedRootNeutralState
    Write-Output '已归还 shared root 到 main，并清空分支租约。'
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

    if ($null -eq $occupancy) {
        return [PSCustomObject]@{
            Applies   = $true
            CanUse    = ($remainingDirty.Count -eq 0)
            Reason    = if ($remainingDirty.Count -eq 0) { 'shared root 未发现 occupancy 文档，lease 校验暂时跳过。' } else { Get-RemainingDirtyGateMessage -Entries $remainingDirty }
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
            $remaining += [PSCustomObject]@{
                Path     = $entry.Path
                Status   = $entry.Status
                Category = '已知本地噪音'
            }
            continue
        }

        if (Test-SharedRootLeaseRuntimeDirtyPath -Path $entry.Path -Branch $branch -Occupancy $occupancy) {
            $remaining += [PSCustomObject]@{
                Path     = $entry.Path
                Status   = $entry.Status
                Category = 'shared root 租约运行态脏改'
            }
            continue
        }

        if (Test-AllowedPath -Path $entry.Path -Mode $Mode -ScopeRoots $ScopeRoots -IncludePaths $IncludePaths) {
            $allowed += [PSCustomObject]@{
                Path     = $entry.Path
                Status   = $entry.Status
                Category = Get-PathGroup -Path $entry.Path
            }
        }
        else {
            $remaining += [PSCustomObject]@{
                Path     = $entry.Path
                Status   = $entry.Status
                Category = Get-PathGroup -Path $entry.Path
            }
        }
    }

    $canContinue = $true
    $reason = '允许继续'

    $sharedRootLease = Test-TaskSharedRootLease -RepoRoot $repoRoot -Branch $branch -OwnerThread $OwnerThread -RemainingEntries $remaining

    if ($Mode -eq 'task' -and $branch -eq 'main') {
        $canContinue = $false
        $reason = "FATAL: OwnerThread '$OwnerThread' 当前位于 main，禁止以 task 模式继续写入；请先切到匹配该线程的 codex/ 分支。"
    }
    elseif ($Mode -eq 'task' -and -not $branch.ToLowerInvariant().StartsWith('codex/')) {
        $canContinue = $false
        $reason = "FATAL: 当前分支 '$branch' 不是 codex/ 前缀，真实任务禁止继续写入。"
    }
    elseif ($Mode -eq 'task' -and -not (Test-BranchOwnedByThread -Branch $branch -OwnerThread $OwnerThread)) {
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

    return [PSCustomObject]@{
        OwnerThread     = $OwnerThread
        RepoRoot        = $repoRoot
        Branch          = $branch
        Head            = $head
        Upstream        = $upstream
        Allowed         = @($allowed)
        Remaining       = @($remaining)
        SharedRootLease = $sharedRootLease
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

    if (@($Report.Allowed).Count -eq 0) {
        Write-Output '- （无）'
    }
    else {
        foreach ($item in $Report.Allowed) {
            Write-Output "- [$($item.Status)] $($item.Path) <$($item.Category)>"
        }
    }

    Write-Output ''
    Write-Output '本轮不会自动纳入同步、只做保留/报告的改动:'

    if (@($Report.Remaining).Count -eq 0) {
        Write-Output '- （无）'
    }
    else {
        foreach ($item in $Report.Remaining) {
            Write-Output "- [$($item.Status)] $($item.Path) <$($item.Category)>"
        }
    }
}

function Get-NextCommitMessage {
    $prefix = Get-Date -Format 'yyyy.MM.dd'
    $since = (Get-Date).ToString('yyyy-MM-dd') + ' 00:00'
    $messages = (Invoke-Git -Arguments @('log', "--since=$since", '--format=%s')).Output
    $numbers = @()

    foreach ($message in $messages) {
        if ($message -match "^$([regex]::Escape($prefix))-(\d{2})$") {
            $numbers += [int]$Matches[1]
        }
    }

    $next = 1
    if ($numbers.Count -gt 0) {
        $next = (($numbers | Measure-Object -Maximum).Maximum) + 1
    }

    return ('{0}-{1}' -f $prefix, ('{0:D2}' -f [int]$next))
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

    $entries = @(Get-BlockingStatusEntries -Entries (Get-StatusEntries) -Branch $branch -Occupancy $occupancy)
    if ($entries.Count -gt 0) {
        throw "当前工作树不干净，不能安全创建/切换到 $TargetBranch。请先收口或隔离现有脏改。"
    }

    $upstream = Get-UpstreamCounts
    if ($branch -eq 'main' -and (($upstream.Ahead -ne 0) -or ($upstream.Behind -ne 0))) {
        throw "当前 main 与远端不同步（behind=$($upstream.Behind), ahead=$($upstream.Ahead)），请先同步后再创建任务分支。"
    }

    $existing = Invoke-Git -Arguments @('branch', '--list', $TargetBranch)
    if ($existing.Output.Count -gt 0 -and -not [string]::IsNullOrWhiteSpace(($existing.Output -join '').Trim())) {
        Invoke-Git -Arguments @('checkout', $TargetBranch) | Out-Null
        Write-Output "已切换到现有分支：$TargetBranch"
    }
    else {
        Invoke-Git -Arguments @('checkout', '-b', $TargetBranch) | Out-Null
        Write-Output "已创建并切换到新分支：$TargetBranch"
    }

    if ((Test-IsSharedRootWorkspace -RepoRoot $repoRoot -OccupancyRecord $occupancy) -and $branch -eq 'main') {
        Set-SharedRootTaskActive -OwnerThread $OwnerThread -TargetBranch $TargetBranch
        Write-Output "已消费 shared root 分支租约，并登记为 task-active：$OwnerThread -> $TargetBranch"
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

            $message = Get-NextCommitMessage
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

        if ($report.Remaining.Count -gt 0) {
            Write-Output ''
            Write-Output '仍保留在工作树中的其他改动:'
            foreach ($item in $report.Remaining) {
                Write-Output "- [$($item.Status)] $($item.Path) <$($item.Category)>"
            }
        }
    }
}
