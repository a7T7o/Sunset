Set-StrictMode -Version Latest

$script:AClassExactPaths = @(
    'Assets/000_Scenes/Primary.unity',
    'ProjectSettings/TagManager.asset',
    'Assets/Editor/StaticObjectOrderAutoCalibrator.cs'
)

$script:BClassExactPaths = @(
    'Assets/YYY_Scripts/Controller/Input/GameInputManager.cs'
)

function Get-StateRepoRoot {
    $repoRoot = Split-Path -Parent (Split-Path -Parent (Split-Path $PSScriptRoot))
    return [System.IO.Path]::GetFullPath($repoRoot)
}

function Get-StateRoot {
    param(
        [string]$RepoRoot = (Get-StateRepoRoot)
    )

    return (Join-Path $RepoRoot '.kiro\state')
}

function Get-ActiveThreadsDirectory {
    param(
        [string]$RepoRoot = (Get-StateRepoRoot)
    )

    return (Join-Path (Get-StateRoot -RepoRoot $RepoRoot) 'active-threads')
}

function Ensure-StateDirectories {
    param(
        [string]$RepoRoot = (Get-StateRepoRoot)
    )

    $stateRoot = Get-StateRoot -RepoRoot $RepoRoot
    $activeThreads = Get-ActiveThreadsDirectory -RepoRoot $RepoRoot
    New-Item -ItemType Directory -Force -Path $stateRoot, $activeThreads | Out-Null
}

function Write-Utf8NoBom {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Path,

        [Parameter(Mandatory = $true)]
        [string]$Content
    )

    $utf8 = [System.Text.UTF8Encoding]::new($false)
    [System.IO.File]::WriteAllText($Path, $Content, $utf8)
}

function Get-GitText {
    param(
        [Parameter(Mandatory = $true)]
        [string]$RepoRoot,

        [Parameter(Mandatory = $true)]
        [string[]]$Arguments
    )

    $output = & git -C $RepoRoot @Arguments 2>$null
    if ($LASTEXITCODE -ne 0 -or $null -eq $output) {
        return ''
    }

    return (($output | Select-Object -First 1).ToString().Trim())
}

function Get-StateCurrentBranch {
    param(
        [string]$RepoRoot = (Get-StateRepoRoot)
    )

    return (Get-GitText -RepoRoot $RepoRoot -Arguments @('branch', '--show-current'))
}

function Get-StateHead {
    param(
        [string]$RepoRoot = (Get-StateRepoRoot)
    )

    return (Get-GitText -RepoRoot $RepoRoot -Arguments @('rev-parse', 'HEAD'))
}

function Get-StateTimestamp {
    return (Get-Date).ToString('o')
}

function Convert-ToRelativeRepoPath {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Path,

        [string]$RepoRoot = (Get-StateRepoRoot)
    )

    $repoRootFull = [System.IO.Path]::GetFullPath($RepoRoot).TrimEnd('\')
    $trimmed = $Path.Trim()

    if ([string]::IsNullOrWhiteSpace($trimmed)) {
        throw 'Path 不能为空。'
    }

    $candidate = if ([System.IO.Path]::IsPathRooted($trimmed)) {
        [System.IO.Path]::GetFullPath($trimmed)
    }
    else {
        [System.IO.Path]::GetFullPath((Join-Path $repoRootFull $trimmed))
    }

    if (-not $candidate.StartsWith($repoRootFull, [System.StringComparison]::OrdinalIgnoreCase)) {
        throw "路径不在仓库内：$Path"
    }

    if ($candidate.Length -eq $repoRootFull.Length) {
        return '.'
    }

    return $candidate.Substring($repoRootFull.Length + 1).Replace('\', '/').TrimEnd('/')
}

function Convert-ToNormalizedPathSet {
    param(
        [string[]]$Paths,

        [string]$RepoRoot = (Get-StateRepoRoot)
    )

    $seen = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::OrdinalIgnoreCase)
    $result = [System.Collections.Generic.List[string]]::new()

    foreach ($path in (Expand-DelimitedStringList -Values $Paths)) {
        if ([string]::IsNullOrWhiteSpace($path)) {
            continue
        }

        $relative = Convert-ToRelativeRepoPath -Path $path -RepoRoot $RepoRoot
        if ($seen.Add($relative)) {
            $result.Add($relative)
        }
    }

    return ,([string[]]@($result))
}

function Expand-DelimitedStringList {
    param(
        [string[]]$Values
    )

    $result = [System.Collections.Generic.List[string]]::new()

    foreach ($value in @($Values)) {
        if ([string]::IsNullOrWhiteSpace($value)) {
            continue
        }

        foreach ($part in ($value -split '[;,]')) {
            if (-not [string]::IsNullOrWhiteSpace($part)) {
                $result.Add($part.Trim())
            }
        }
    }

    return [string[]]@($result)
}

function Get-ThreadStatePath {
    param(
        [Parameter(Mandatory = $true)]
        [string]$ThreadName,

        [string]$RepoRoot = (Get-StateRepoRoot)
    )

    $sanitized = $ThreadName.Trim()
    foreach ($invalid in [System.IO.Path]::GetInvalidFileNameChars()) {
        $sanitized = $sanitized.Replace($invalid, '_')
    }

    $sanitized = $sanitized.Replace(' ', '_')
    return (Join-Path (Get-ActiveThreadsDirectory -RepoRoot $RepoRoot) ($sanitized + '.json'))
}

function Read-ThreadState {
    param(
        [Parameter(Mandatory = $true)]
        [string]$ThreadName,

        [string]$RepoRoot = (Get-StateRepoRoot)
    )

    $path = Get-ThreadStatePath -ThreadName $ThreadName -RepoRoot $RepoRoot
    if (-not (Test-Path -LiteralPath $path)) {
        return $null
    }

    $raw = Get-Content -Raw -LiteralPath $path -Encoding UTF8
    return ($raw | ConvertFrom-Json)
}

function Write-ThreadState {
    param(
        [Parameter(Mandatory = $true)]
        [object]$State,

        [string]$RepoRoot = (Get-StateRepoRoot)
    )

    Ensure-StateDirectories -RepoRoot $RepoRoot

    $cleanOwnedPaths = Convert-ToNormalizedPathSet -Paths @($State.owned_paths) -RepoRoot $RepoRoot
    $cleanCarriedForeignPaths = Convert-ToNormalizedPathSet -Paths @($State.carried_foreign_paths) -RepoRoot $RepoRoot
    $cleanSharedFiles = Convert-ToNormalizedPathSet -Paths @($State.shared_files) -RepoRoot $RepoRoot
    $cleanTouchpoints = @(Expand-DelimitedStringList -Values @($State.touched_touchpoints) | Select-Object -Unique)
    $cleanExpectedSyncPaths = Convert-ToNormalizedPathSet -Paths @($State.expected_sync_paths) -RepoRoot $RepoRoot
    $cleanAClassLockedPaths = Convert-ToNormalizedPathSet -Paths @($State.a_class_locked_paths) -RepoRoot $RepoRoot
    $cleanBlockers = @($State.blockers | Where-Object { -not [string]::IsNullOrWhiteSpace($_) } | ForEach-Object { $_.ToString().Trim() })

    $stateObject = [ordered]@{
        schema_version        = 1
        thread                = [string]$State.thread
        status                = [string]$State.status
        workdir               = [string]$State.workdir
        current_branch        = [string]$State.current_branch
        base_head             = [string]$State.base_head
        current_slice         = [string]$State.current_slice
        owned_paths           = $cleanOwnedPaths
        carried_foreign_paths = $cleanCarriedForeignPaths
        shared_files          = $cleanSharedFiles
        touched_touchpoints   = $cleanTouchpoints
        expected_sync_paths   = $cleanExpectedSyncPaths
        a_class_locked_paths  = $cleanAClassLockedPaths
        blockers              = $cleanBlockers
        updated_at            = [string]$State.updated_at
    }

    $json = $stateObject | ConvertTo-Json -Depth 6
    $path = Get-ThreadStatePath -ThreadName $stateObject.thread -RepoRoot $RepoRoot
    Write-Utf8NoBom -Path $path -Content $json
    return $path
}

function Remove-ThreadState {
    param(
        [Parameter(Mandatory = $true)]
        [string]$ThreadName,

        [string]$RepoRoot = (Get-StateRepoRoot)
    )

    $path = Get-ThreadStatePath -ThreadName $ThreadName -RepoRoot $RepoRoot
    if (Test-Path -LiteralPath $path) {
        Remove-Item -LiteralPath $path -Force
    }
}

function Get-AllThreadStates {
    param(
        [string]$RepoRoot = (Get-StateRepoRoot)
    )

    $dir = Get-ActiveThreadsDirectory -RepoRoot $RepoRoot
    if (-not (Test-Path -LiteralPath $dir)) {
        return @()
    }

    $states = @()
    foreach ($file in Get-ChildItem -LiteralPath $dir -Filter '*.json' -File) {
        $raw = Get-Content -Raw -LiteralPath $file.FullName -Encoding UTF8
        $states += ($raw | ConvertFrom-Json)
    }

    return @($states)
}

function Test-AClassPath {
    param([string]$RelativePath)

    $path = $RelativePath.Replace('\', '/')
    $extension = [System.IO.Path]::GetExtension($path)

    if ($script:AClassExactPaths -contains $path) {
        return $true
    }

    if ($extension -eq '.unity' -or $extension -eq '.prefab') {
        return $true
    }

    if ($path.StartsWith('ProjectSettings/', [System.StringComparison]::OrdinalIgnoreCase) -and $extension -eq '.asset') {
        return $true
    }

    return $false
}

function Test-BClassPath {
    param([string]$RelativePath)

    $path = $RelativePath.Replace('\', '/')
    return ($script:BClassExactPaths -contains $path)
}

function Get-PathClass {
    param([string]$RelativePath)

    if (Test-BClassPath -RelativePath $RelativePath) {
        return 'B'
    }

    if (Test-AClassPath -RelativePath $RelativePath) {
        return 'A'
    }

    return 'C'
}

function Get-TouchpointIntersection {
    param(
        [string[]]$Left,
        [string[]]$Right
    )

    $leftSet = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::OrdinalIgnoreCase)
    foreach ($item in (Expand-DelimitedStringList -Values $Left)) {
        if (-not [string]::IsNullOrWhiteSpace($item)) {
            [void]$leftSet.Add($item.Trim())
        }
    }

    $intersection = [System.Collections.Generic.List[string]]::new()
    foreach ($item in (Expand-DelimitedStringList -Values $Right)) {
        if ([string]::IsNullOrWhiteSpace($item)) {
            continue
        }

        $normalized = $item.Trim()
        if ($leftSet.Contains($normalized)) {
            $intersection.Add($normalized)
        }
    }

    return ,([string[]]@($intersection))
}

function Get-BClassConflicts {
    param(
        [Parameter(Mandatory = $true)]
        [string]$ThreadName,

        [string[]]$SharedFiles,
        [string[]]$TouchedTouchpoints,

        [string]$RepoRoot = (Get-StateRepoRoot)
    )

    $sharedSet = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::OrdinalIgnoreCase)
    foreach ($path in (Expand-DelimitedStringList -Values $SharedFiles)) {
        if (-not [string]::IsNullOrWhiteSpace($path)) {
            [void]$sharedSet.Add($path)
        }
    }

    $conflicts = [System.Collections.Generic.List[string]]::new()
    foreach ($state in (Get-AllThreadStates -RepoRoot $RepoRoot)) {
        if ($state.thread -eq $ThreadName) {
            continue
        }

        if ($state.status -notin @('ACTIVE', 'READY_TO_SYNC')) {
            continue
        }

        $otherShared = @(Expand-DelimitedStringList -Values @($state.shared_files))
        foreach ($path in $otherShared) {
            if (-not $sharedSet.Contains($path)) {
                continue
            }

            $intersection = Get-TouchpointIntersection -Left $TouchedTouchpoints -Right @($state.touched_touchpoints)
            if ($intersection.Count -gt 0) {
                $conflicts.Add("$($state.thread) 正在占用 $path 的同一触点：$($intersection -join ', ')")
                continue
            }

            if (@($TouchedTouchpoints).Count -eq 0 -or @($state.touched_touchpoints).Count -eq 0) {
                $conflicts.Add("$($state.thread) 正在占用共享文件 $path，但至少有一侧缺少触点声明，按保守口径阻断。")
            }
        }
    }

    return ,([string[]]@($conflicts))
}

function Get-DefaultExpectedSyncPaths {
    param(
        [string[]]$OwnedPaths,
        [string[]]$CarriedForeignPaths,
        [string[]]$SharedFiles,

        [string]$RepoRoot = (Get-StateRepoRoot)
    )

    return ,([string[]](Convert-ToNormalizedPathSet -Paths (@($OwnedPaths) + @($CarriedForeignPaths) + @($SharedFiles)) -RepoRoot $RepoRoot))
}

function Acquire-StateExecutionLock {
    param(
        [Parameter(Mandatory = $true)]
        [string]$LockName,

        [int]$TimeoutSeconds = 30,

        [string]$RepoRoot = (Get-StateRepoRoot)
    )

    Ensure-StateDirectories -RepoRoot $RepoRoot
    $lockPath = Join-Path (Get-StateRoot -RepoRoot $RepoRoot) ($LockName + '.lock')
    $deadline = (Get-Date).AddSeconds($TimeoutSeconds)

    while ($true) {
        try {
            $stream = [System.IO.File]::Open($lockPath, [System.IO.FileMode]::CreateNew, [System.IO.FileAccess]::Write, [System.IO.FileShare]::None)
            $payload = [System.Text.UTF8Encoding]::new($false).GetBytes((Get-StateTimestamp))
            $stream.Write($payload, 0, $payload.Length)
            $stream.Flush()
            return [PSCustomObject]@{
                Path   = $lockPath
                Stream = $stream
            }
        }
        catch {
            if ((Get-Date) -ge $deadline) {
                throw "等待执行锁超时：$lockPath"
            }

            Start-Sleep -Milliseconds 250
        }
    }
}

function Release-StateExecutionLock {
    param(
        [Parameter(Mandatory = $true)]
        [object]$LockHandle
    )

    if ($null -ne $LockHandle.Stream) {
        $LockHandle.Stream.Dispose()
    }

    if (Test-Path -LiteralPath $LockHandle.Path) {
        Remove-Item -LiteralPath $LockHandle.Path -Force
    }
}

function Invoke-StableGitSafeSync {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Action,

        [Parameter(Mandatory = $true)]
        [string]$OwnerThread,

        [string]$Mode = 'task',

        [string[]]$IncludePaths = @(),

        [string[]]$ScopeRoots = @(),

        [string]$RepoRoot = (Get-StateRepoRoot)
    )

    $launcherPath = 'C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1'
    if (-not (Test-Path -LiteralPath $launcherPath)) {
        throw "稳定 launcher 不存在：$launcherPath"
    }

    $normalizedIncludePaths = @(Expand-DelimitedStringList -Values $IncludePaths | Where-Object { -not [string]::IsNullOrWhiteSpace($_) })
    $normalizedScopeRoots = @(Expand-DelimitedStringList -Values $ScopeRoots | Where-Object { -not [string]::IsNullOrWhiteSpace($_) })

    $useWorkingTreeGitSafeSync = @($normalizedIncludePaths + $normalizedScopeRoots) -contains 'scripts/git-safe-sync.ps1'
    $targetScriptPath = $launcherPath
    if ($useWorkingTreeGitSafeSync) {
        $repoScriptPath = Join-Path $RepoRoot 'scripts\git-safe-sync.ps1'
        if (-not (Test-Path -LiteralPath $repoScriptPath)) {
            throw "仓库内 git-safe-sync 脚本不存在：$repoScriptPath"
        }

        $targetScriptPath = $repoScriptPath
    }

    $output = @()
    $arguments = @(
        '-NoProfile',
        '-ExecutionPolicy', 'Bypass',
        '-File', $targetScriptPath,
        '-Action', $Action,
        '-OwnerThread', $OwnerThread,
        '-Mode', $Mode
    )

    if (-not $useWorkingTreeGitSafeSync) {
        $arguments += @('-RepoRoot', $RepoRoot)
    }

    if ($normalizedIncludePaths.Count -gt 0) {
        $arguments += @('-IncludePaths', ($normalizedIncludePaths -join ';'))
    }

    if ($normalizedScopeRoots.Count -gt 0) {
        $arguments += @('-ScopeRoots', ($normalizedScopeRoots -join ';'))
    }

    $output = & powershell @arguments 2>&1
    $exitCode = if ($null -ne $LASTEXITCODE) { [int]$LASTEXITCODE } else { 0 }

    if ($Action -eq 'preflight' -and $exitCode -eq 0) {
        $reportedFailure = $false
        foreach ($line in @($output | ForEach-Object { $_.ToString() })) {
            if ([string]::IsNullOrWhiteSpace($line)) {
                continue
            }

            if ($line -match 'CanContinue=False' -or
                $line -match '是否允许按当前模式继续:\s*False' -or
                $line -match '^判断原因:\s*FATAL:') {
                $reportedFailure = $true
                break
            }
        }

        if ($reportedFailure) {
            $exitCode = 2
        }
    }

    return [PSCustomObject]@{
        ExitCode = $exitCode
        Output   = @($output | ForEach-Object { $_.ToString() })
    }
}

function Get-UsefulBlockersFromOutput {
    param([string[]]$OutputLines)

    $patterns = @(
        'FATAL:',
        'CanContinue=False',
        '第一真实 blocker',
        'blocked',
        'compile',
        '代码闸门',
        'remaining dirty',
        'own roots'
    )

    $picked = [System.Collections.Generic.List[string]]::new()
    foreach ($line in @($OutputLines)) {
        if ([string]::IsNullOrWhiteSpace($line)) {
            continue
        }

        foreach ($pattern in $patterns) {
            if ($line.IndexOf($pattern, [System.StringComparison]::OrdinalIgnoreCase) -ge 0) {
                $picked.Add($line.Trim())
                break
            }
        }
    }

    if ($picked.Count -gt 0) {
        return ,([string[]]@($picked | Select-Object -Unique))
    }

    return ,([string[]]@($OutputLines | Where-Object { -not [string]::IsNullOrWhiteSpace($_) } | Select-Object -Last 8))
}

function Release-AClassLocksForState {
    param(
        [Parameter(Mandatory = $true)]
        [object]$State,

        [string]$ReleaseNote = 'parked',

        [string]$RepoRoot = (Get-StateRepoRoot)
    )

    $releaseScript = Join-Path $RepoRoot '.kiro\scripts\locks\Release-Lock.ps1'
    if (-not (Test-Path -LiteralPath $releaseScript)) {
        throw "Release-Lock.ps1 不存在：$releaseScript"
    }

    foreach ($path in @($State.a_class_locked_paths)) {
        & powershell -NoProfile -ExecutionPolicy Bypass -File $releaseScript -TargetPath $path -OwnerThread $State.thread -ReleaseNote $ReleaseNote | Out-Null
        if ($LASTEXITCODE -ne 0) {
            throw "释放 A 类锁失败：$path"
        }
    }
}
