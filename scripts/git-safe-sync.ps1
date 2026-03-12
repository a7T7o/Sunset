param(
    [Parameter(Mandatory = $true)]
    [ValidateSet('preflight', 'ensure-branch', 'sync')]
    [string]$Action,

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
        '.kiro/hooks',
        '.kiro/specs/Steering规则区优化'
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
    if ($Path -like '.kiro/specs/Steering规则区优化/*' -or $Path -like '.kiro/steering/*' -or $Path -like '.kiro/hooks/*' -or $Path -eq '.gitattributes' -or $Path -eq '.gitignore' -or $Path -eq 'AGENTS.md' -or $Path -like 'scripts/*') { return '治理线改动' }
    if ($Path -like '.kiro/specs/农田系统/*') { return '农田线改动' }
    if ($Path -like '.kiro/specs/900_开篇/*') { return '开篇线改动' }
    if ($Path -like '.kiro/about/*') { return 'about治理线改动' }
    if ($Path -like '.codex/threads/*') { return '线程记忆改动' }
    return '其他改动'
}

function New-PreflightReport {
    param(
        [string]$Mode,
        [string[]]$ScopeRoots,
        [string[]]$IncludePaths
    )

    $branch = Get-CurrentBranch
    $head = Get-HeadShort
    $upstream = Get-UpstreamCounts
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

    if ($Mode -eq 'task' -and $branch -eq 'main') {
        $canContinue = $false
        $reason = '当前在 main，上真实任务前必须先创建 codex/ 任务分支。'
    }
    elseif ($Mode -eq 'task' -and -not $branch.StartsWith('codex/')) {
        $canContinue = $false
        $reason = '真实任务分支必须使用 codex/ 前缀。'
    }
    elseif ($Mode -eq 'task' -and ((@(Expand-PathList -Paths $ScopeRoots).Count -eq 0) -and (@(Expand-PathList -Paths $IncludePaths).Count -eq 0))) {
        $canContinue = $false
        $reason = 'task 模式必须提供 ScopeRoots 或 IncludePaths，不能无边界自动提交。'
    }

    return [PSCustomObject]@{
        Branch      = $branch
        Head        = $head
        Upstream    = $upstream
        Allowed     = @($allowed)
        Remaining   = @($remaining)
        CanContinue = $canContinue
        Reason      = $reason
    }
}

function Write-PreflightReport {
    param($Report)

    Write-Output "当前分支: $($Report.Branch)"
    Write-Output "当前 HEAD: $($Report.Head)"
    Write-Output "upstream 状态 ($($Report.Upstream.Label)): behind=$($Report.Upstream.Behind), ahead=$($Report.Upstream.Ahead)"
    Write-Output "是否允许按当前模式继续: $($Report.CanContinue)"
    Write-Output "判断原因: $($Report.Reason)"
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
    param([string]$TargetBranch)

    $branch = Get-CurrentBranch
    if ($branch -eq $TargetBranch) {
        Write-Output "已位于目标分支：$TargetBranch"
        return
    }

    $entries = @(Get-StatusEntries | Where-Object { -not (Test-NoisePath -Path $_.Path) })
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
}

$repoRoot = Get-RepoRoot
Set-Location $repoRoot

switch ($Action) {
    'preflight' {
        $report = New-PreflightReport -Mode $Mode -ScopeRoots $ScopeRoots -IncludePaths $IncludePaths
        Write-PreflightReport -Report $report
    }

    'ensure-branch' {
        if ([string]::IsNullOrWhiteSpace($BranchName)) {
            throw 'ensure-branch 必须提供 -BranchName。'
        }

        Ensure-TaskBranch -TargetBranch $BranchName
    }

    'sync' {
        $report = New-PreflightReport -Mode $Mode -ScopeRoots $ScopeRoots -IncludePaths $IncludePaths
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



