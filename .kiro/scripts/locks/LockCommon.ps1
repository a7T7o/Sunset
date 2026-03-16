Set-StrictMode -Version Latest

function Get-LockRepoRoot {
    $repoRoot = Split-Path -Parent (Split-Path -Parent (Split-Path -Parent $PSScriptRoot))
    return [System.IO.Path]::GetFullPath($repoRoot)
}

function Get-LockDirectories {
    param(
        [string]$RepoRoot = (Get-LockRepoRoot)
    )

    $root = Join-Path $RepoRoot '.kiro\locks'
    return [PSCustomObject]@{
        Root    = $root
        Active  = Join-Path $root 'active'
        History = Join-Path $root 'history'
    }
}

function Ensure-LockDirectories {
    param(
        [string]$RepoRoot = (Get-LockRepoRoot)
    )

    $dirs = Get-LockDirectories -RepoRoot $RepoRoot
    New-Item -ItemType Directory -Force -Path $dirs.Root, $dirs.Active, $dirs.History | Out-Null
    return $dirs
}

function Get-GitText {
    param(
        [Parameter(Mandatory = $true)]
        [string]$RepoRoot,

        [Parameter(Mandatory = $true)]
        [string[]]$Arguments
    )

    $output = & git -C $RepoRoot @Arguments 2>$null
    if ($LASTEXITCODE -ne 0) {
        return ''
    }

    if ($null -eq $output) {
        return ''
    }

    return (($output | Select-Object -First 1).ToString().Trim())
}

function Resolve-LockTarget {
    param(
        [Parameter(Mandatory = $true)]
        [string]$TargetPath,

        [string]$RepoRoot = (Get-LockRepoRoot)
    )

    $repoRootFull = [System.IO.Path]::GetFullPath($RepoRoot).TrimEnd('\')
    $fullPath = if ([System.IO.Path]::IsPathRooted($TargetPath)) {
        [System.IO.Path]::GetFullPath($TargetPath)
    }
    else {
        [System.IO.Path]::GetFullPath((Join-Path $repoRootFull $TargetPath))
    }

    if (-not $fullPath.StartsWith($repoRootFull, [System.StringComparison]::OrdinalIgnoreCase)) {
        throw "Target path is outside the repo: $fullPath"
    }

    if (-not (Test-Path -LiteralPath $fullPath)) {
        throw "Target path does not exist: $fullPath"
    }

    $relative = $fullPath.Substring($repoRootFull.Length + 1).Replace('\', '/')
    $key = ($relative -replace '[\\/: ]+', '__')
    $dirs = Get-LockDirectories -RepoRoot $repoRootFull
    $fileName = "A__{0}.lock.json" -f $key

    return [PSCustomObject]@{
        RepoRoot       = $repoRootFull
        TargetFullPath = $fullPath
        RelativePath   = $relative
        Key            = $key
        LockFileName   = $fileName
        ActiveLockPath = Join-Path $dirs.Active $fileName
    }
}

function Read-LockFile {
    param(
        [Parameter(Mandatory = $true)]
        [string]$LockPath
    )

    if (-not (Test-Path -LiteralPath $LockPath)) {
        return $null
    }

    $raw = Get-Content -Raw -LiteralPath $LockPath -Encoding UTF8
    return $raw | ConvertFrom-Json
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

function Convert-LockToJson {
    param(
        [Parameter(Mandatory = $true)]
        [object]$LockObject
    )

    return ($LockObject | ConvertTo-Json -Depth 8)
}
